using UnityEngine;

namespace Anomalus.Pathfinding.MapObjects
{
    public interface IPathMapObject
    {
        Vector2Int[] GetMovablePoints(PathMap map);
        Vector2Int[] GetBlockedPoints(PathMap map);
    }
}
