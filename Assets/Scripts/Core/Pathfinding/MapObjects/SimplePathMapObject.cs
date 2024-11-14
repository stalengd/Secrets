using UnityEngine;

namespace Anomalus.Pathfinding.MapObjects
{
    public class SimplePathMapObject : PathMapObjectBase
    {
        [SerializeField] private Rect _movableZone;
        [SerializeField] private Rect _blockedZone;

        private PathMap _map;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                _map.UpdateObjectPoints(this);
            }
        }
        private bool _isEnabled = true;

        private void OnDestroy()
        {
            if (_map == null) return;
            _map.DeleteObjectPoints(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube((Vector3)_movableZone.center + transform.position, _movableZone.size);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector3)_blockedZone.center + transform.position, _blockedZone.size);

            if (_map is { })
            {
                foreach (var point in GetMovablePoints(_map))
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(_map.ToWorldPosition(point), _map.Grid.cellSize);
                }
            }
        }

        public override Vector2Int[] GetMovablePoints(PathMap map)
        {
            _map = map;

            if (!IsEnabled)
                return new Vector2Int[0];

            return GetPointsFromRect(_movableZone);
        }

        public override Vector2Int[] GetBlockedPoints(PathMap map)
        {
            _map = map;

            if (!IsEnabled)
                return new Vector2Int[0];

            return GetPointsFromRect(_blockedZone);
        }

        private Vector2Int[] GetPointsFromRect(Rect rect)
        {
            var point = _map.ToMapPoint(rect.min + (Vector2)transform.position);
            var size = new Vector2Int(Mathf.RoundToInt(rect.size.x), Mathf.RoundToInt(rect.size.y));
            var points = new Vector2Int[size.x * size.y];

            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    points[x + size.x * y] = new Vector2Int(point.x + x, point.y + y);
                }
            }

            return points;
        }
    }
}
