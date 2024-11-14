using UnityEngine;

namespace Anomalus.Pathfinding.MapObjects
{
    public abstract class PathMapObjectBase : MonoBehaviour, IPathMapObject
    {
        public abstract Vector2Int[] GetBlockedPoints(PathMap map);
        public abstract Vector2Int[] GetMovablePoints(PathMap map);
    }
}
