using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Anomalus.Pathfinding
{
    public class PathFindTest : MonoBehaviour
    {
        [SerializeField] private Grid _grid;

        [Inject] private readonly Pathfinder _pathfinder;

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                var from = (Vector2Int)_grid.WorldToCell(transform.position);
                var to = (Vector2Int)_grid.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.value));
                _pathfinder.FindPath(from, to, path =>
                {
                    if (!path.IsFull)
                    {
                        Debug.Log($"No path found between {from} & {to}");
                        return;
                    }

                    var worldPoints = _pathfinder.Map.PointsToWorld(path.Points);
                    for (var i = 0; i < worldPoints.Count() - 1; i++)
                    {
                        Debug.DrawLine(worldPoints.ElementAt(i), worldPoints.ElementAt(i + 1), Color.green, 1f);
                    }
                });
            }
        }
    }
}
