using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Priority_Queue;
using UnityEngine;
using VContainer;

namespace Anomalus.Pathfinding
{
    public sealed class Pathfinder : IDisposable
    {
        [Inject] private readonly PathMap _map;

        public PathMap Map => _map;

        private static readonly float MaxCost = 50f;
        private static readonly float MaxPriority = 60f;
        private static readonly FastPriorityQueue<Node> OpenNodesQueue = new(2048);
        private static readonly Dictionary<Vector2Int, Node> Nodes = new(2048);
        private static readonly Vector2Int[] NeighbourOffsets = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };
        private static readonly Queue<PathfindingRequest> Requests = new();
        private static readonly List<PathfindingResult> Results = new();
        private static readonly List<PathfindingResult> ResultsBuffer = new();
        private static readonly CancellationTokenSource CancellationTokenSource = new();
        private static Vector2Int _startPoint;
        private static Vector2Int _endPoint;
        private static Task _workerTask;

        public void Update()
        {
            if (_workerTask is { IsFaulted: true })
            {
                Debug.LogException(new Exception("Pathfinder is failed", _workerTask.Exception));
                _workerTask = null;
            }

            ResultsBuffer.Clear();
            lock (Results)
            {
                ResultsBuffer.AddRange(Results);
                Results.Clear();
            }

            foreach (var result in ResultsBuffer)
            {
                var request = result.Request;

                request.Callback(result.Path);
            }
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
            lock (Requests)
            {
                Requests.Clear();
            }
            lock (Results)
            {
                Results.Clear();
            }
        }

        public void FindPath(Vector2Int from, Vector2Int to, Action<Path> callback, List<Vector2Int> points = null)
        {
            EnqueuePathRequest(new PathfindingRequest(from, to, Map, callback, points ?? new()));
        }

        public void FindPath(Vector3 from, Vector3 to, Action<Path> callback, List<Vector2Int> points = null)
        {
            var fromPoint = (Vector2Int)Map.Grid.WorldToCell(from);
            var toPoint = (Vector2Int)Map.Grid.WorldToCell(to);
            FindPath(fromPoint, toPoint, callback, points);
        }

        private static void EnqueuePathRequest(PathfindingRequest request)
        {
            lock (Requests)
            {
                Requests.Enqueue(request);
            }

            if (_workerTask == null || _workerTask.IsCanceled || _workerTask.IsFaulted || _workerTask.IsCompleted)
            {
                _workerTask = Task.Run(FinderWorker, CancellationTokenSource.Token);
            }
        }

        private static async Task FinderWorker()
        {
            UnityEngine.Profiling.Profiler.BeginThreadProfiling("Pathfinding", "Pathfinding worker");

            var token = CancellationTokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                PathfindingRequest request;
                bool hasRequest;

                lock (Requests)
                {
                    hasRequest = Requests.TryDequeue(out request);
                }

                if (hasRequest)
                {
                    var r = FindPath(request);

                    var path =
                        new Path(request.StartPoint, request.EndPoint, r.IsFullPath, request.PointsBuffer);
                    path = BuildPath(path, r.NearestToEndNode);
                    r.Path = path;

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    lock (Results)
                    {
                        Results.Add(r);
                    }
                }
                else
                {
                    await Task.Delay(20); // Great design
                }
            }

            UnityEngine.Profiling.Profiler.EndThreadProfiling();
        }

        private static PathfindingResult FindPath(PathfindingRequest request)
        {
            OpenNodesQueue.Clear();
            Nodes.Clear();
            Node.FreeAll();

            _startPoint = request.StartPoint;
            _endPoint = request.EndPoint;
            var map = request.Map;
            var startNode = Node.Get().Init(0f, _startPoint, default, true);

            AddOpenNode(startNode);

            var shortestSqrDistance = float.PositiveInfinity;
            var nearestToEndNode = startNode;

            while (OpenNodesQueue.Count > 0)
            {
                var node = GetNextNode();
                if (node.Point == _endPoint)
                {
                    return new PathfindingResult(request, true, node);
                }

                var sqrDistance = (_endPoint - node.Point).sqrMagnitude;
                if (shortestSqrDistance > sqrDistance)
                {
                    shortestSqrDistance = sqrDistance;
                    nearestToEndNode = node;
                }

                MarkNodeAsExplored(node);

                if (node.TotalCost >= MaxCost)
                {
                    continue;
                }

                foreach (var offset in NeighbourOffsets)
                {
                    HandleNeighbour(node, offset, map);
                }
            }

            return new PathfindingResult(request, false, nearestToEndNode);
        }


        private static Node GetNextNode()
        {
            var node = OpenNodesQueue.Dequeue();
            return node;
        }

        private static float CalculatePriority(Node node)
        {
            return node.TotalCost + (_endPoint - node.Point).magnitude;
        }

        private static Path BuildPath(Path path, Node endNode)
        {
            path.Points.Add(endNode.Point);

            if (path.From == endNode.Point)
                return path;

            while (true)
            {
                var previous = endNode.PreviousPoint;

                if (previous == path.From)
                    break;

                path.Points.Add(previous);

                endNode = Nodes[previous];
            }

            path.Points.Reverse();
            return path;
        }

        private static void MarkNodeAsExplored(Node node)
        {
            node.IsOpen = false;
        }

        private static void HandleNeighbour(Node currentNode, Vector2Int offset, PathMap map)
        {
            var point = currentNode.Point + offset;

            if (!map.IsPointMovable(point))
            {
                return;
            }

            if (Nodes.TryGetValue(point, out var node))
            {
                if (!node.IsOpen) return;

                var costToNode = currentNode.GetCostToNode(point);
                if (currentNode.TotalCost + costToNode < node.TotalCost)
                {
                    node.TotalCost = currentNode.TotalCost + costToNode;
                    node.PreviousPoint = currentNode.Point;

                    UpdateOpenNode(node);
                }
            }
            else
            {
                var costToNode = currentNode.GetCostToNode(point);
                node = Node.Get().Init(currentNode.TotalCost + costToNode, point, currentNode.Point, true);
                AddOpenNode(node);
            }
        }

        private static bool AddOpenNode(Node node)
        {
            var priority = CalculatePriority(node);
            if (priority >= MaxPriority) return false;

            Nodes.Add(node.Point, node);
            OpenNodesQueue.Enqueue(node, priority);
            return true;
        }

        private static void UpdateOpenNode(Node node)
        {
            var priority = CalculatePriority(node);
            OpenNodesQueue.UpdatePriority(node, priority);
        }

        private readonly struct PathfindingRequest
        {
            public Vector2Int StartPoint { get; }
            public Vector2Int EndPoint { get; }
            public PathMap Map { get; }
            public Action<Path> Callback { get; }
            public List<Vector2Int> PointsBuffer { get; }

            public PathfindingRequest
                (Vector2Int startPoint, Vector2Int endPoint, PathMap map, Action<Path> callback, List<Vector2Int> pointsBuffer)
            {
                StartPoint = startPoint;
                EndPoint = endPoint;
                Map = map;
                Callback = callback;
                PointsBuffer = pointsBuffer;
            }
        }

        private struct PathfindingResult
        {
            public PathfindingRequest Request { get; }
            public bool IsFullPath { get; }
            public Node NearestToEndNode { get; }
            public Path Path { get; set; }

            public PathfindingResult(PathfindingRequest request, bool isFullPath, Node nearestToEndNode)
            {
                Request = request;
                IsFullPath = isFullPath;
                NearestToEndNode = nearestToEndNode;
                Path = default;
            }
        }

        public readonly struct Path
        {
            public Vector2Int From { get; }
            public Vector2Int To { get; }
            public bool IsFull { get; }
            public List<Vector2Int> Points { get; }

            public Path(Vector2Int from, Vector2Int to, bool isFull, List<Vector2Int> points)
            {
                From = from;
                To = to;
                IsFull = isFull;
                Points = points;
            }
        }

        private sealed class Node : FastPriorityQueueNode
        {
            public float TotalCost { get; set; }
            public Vector2Int Point { get; set; }
            public Vector2Int PreviousPoint { get; set; }
            public bool IsOpen { get; set; }

            private readonly static List<Node> Pool = new(2048);
            private static int _poolPosition = 0;

            public Node Init(float totalCost, Vector2Int point, Vector2Int previousPoint, bool isOpen)
            {
                TotalCost = totalCost;
                Point = point;
                PreviousPoint = previousPoint;
                IsOpen = isOpen;
                return this;
            }

            public float GetCostToNode(Vector2Int point)
            {
                return (point - Point).magnitude;
            }

            public static Node Get()
            {
                Node node;
                if (Pool.Count > _poolPosition)
                {
                    node = Pool[_poolPosition];
                }
                else
                {
                    node = new Node();
                    Pool.Add(node);
                }

                _poolPosition++;

                return node;
            }

            public static void FreeAll()
            {
                _poolPosition = 0;
            }
        }
    }
}
