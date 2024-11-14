using System.Collections.Generic;
using System.Linq;
using Anomalus.Pathfinding.MapObjects;
using UnityEngine;

namespace Anomalus.Pathfinding
{
    public sealed class PathMap : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private bool _generateAtStart = false;

        private List<PointsGroup> _pointsGroups = new();

        private readonly struct PointsGroup
        {
            public IPathMapObject Source { get; }
            public HashSet<Vector2Int> MovablePoints { get; }
            public HashSet<Vector2Int> BlockedPoints { get; }

            public PointsGroup(IPathMapObject source)
            {
                Source = source;
                MovablePoints = new HashSet<Vector2Int>();
                BlockedPoints = new HashSet<Vector2Int>();
            }
        }

        public Grid Grid => _grid;
        public BoundsInt Bounds { get; private set; }

        private void Start()
        {
            if (_generateAtStart)
            {
                GenerateMap();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_pointsGroups == null) return;

            Gizmos.color = new Color32(204, 70, 66, 180);
            foreach (var point in GetAllBlockedPoints())
            {
                Gizmos.DrawWireCube(_grid.CellToWorld((Vector3Int)point) + _grid.cellSize * 0.5f, _grid.cellSize);
            }
        }

        [ContextMenu("Generate Map")]
        public void GenerateMap()
        {
            _pointsGroups ??= new List<PointsGroup>();
            _pointsGroups.Clear();

            foreach (var o in FindObjectsByType<PathMapObjectBase>(FindObjectsSortMode.None))
            {
                UpdateObjectPoints(o);
            }
        }

        public bool IsPointMovable(Vector2Int point)
        {
            for (var i = 0; i < _pointsGroups.Count; i++)
            {
                var g = _pointsGroups[i];
                if (g.BlockedPoints.Contains(point))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<Vector3> PointsToWorld(IEnumerable<Vector2Int> points)
        {
            return points.Select(p => _grid.CellToWorld((Vector3Int)p) + _grid.cellSize * 0.5f);
        }

        public Vector3 ToWorldPosition(Vector2Int point)
        {
            return Grid.CellToWorld((Vector3Int)point) + Grid.cellSize * 0.5f;
        }

        public Vector2Int ToMapPoint(Vector3 position)
        {
            return (Vector2Int)Grid.WorldToCell(position);
        }

        public void UpdateObjectPoints(IPathMapObject mapObject)
        {
            var group = new PointsGroup(mapObject);

            //group.MovablePoints.AddRange(mapObject.GetMovablePoints());
            var blockedPoints = mapObject.GetBlockedPoints(this);
            for (var i = 0; i < blockedPoints.Length; i++)
            {
                group.BlockedPoints.Add(blockedPoints[i]);
            }

            for (var i = 0; i < _pointsGroups.Count; i++)
            {
                if (_pointsGroups[i].Source == mapObject)
                {
                    _pointsGroups[i] = group;
                    return;
                }
            }

            _pointsGroups.Add(group);
        }

        public void DeleteObjectPoints(IPathMapObject mapObject)
        {
            for (var i = 0; i < _pointsGroups.Count; i++)
            {
                if (_pointsGroups[i].Source == mapObject)
                {
                    _pointsGroups.RemoveAt(i);
                    return;
                }
            }
        }

        private List<T> GetComponentsRecursive<T>(Transform transform)
        {
            var r = new List<T>();

            GetComponentsRecursive(transform, r);

            return r;
        }

        private void GetComponentsRecursive<T>(Transform transform, List<T> list)
        {
            foreach (Transform t in transform)
            {
                if (t.TryGetComponent<T>(out var component))
                    list.Add(component);

                GetComponentsRecursive(t, list);
            }
        }


        private List<Vector2Int> GetAllMovablePoints()
        {
            //var r = new List<Vector2Int>();
            //foreach (var group in pointsGroups)
            //{
            //    r.AddRange(group.MovablePoints);
            //}
            //return r;

            var r = new List<Vector2Int>();
            foreach (var pos in Bounds.allPositionsWithin)
            {
                r.Add((Vector2Int)pos);
            }
            return r;
        }

        private List<Vector2Int> GetAllBlockedPoints()
        {
            var r = new List<Vector2Int>();
            foreach (var group in _pointsGroups)
            {
                r.AddRange(group.BlockedPoints);
            }
            return r;
        }
    }
}
