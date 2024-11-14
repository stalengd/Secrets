using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Anomalus.Pathfinding.MapObjects
{
    public class TilePathMapObject : PathMapObjectBase
    {
        [SerializeField] protected Tilemap Tilemap;

        public override Vector2Int[] GetMovablePoints(PathMap map)
        {
            return new Vector2Int[0];
        }

        public override Vector2Int[] GetBlockedPoints(PathMap map)
        {
            var width = Tilemap.cellBounds.size.x;

            var i = 0;
            var tiles = new Dictionary<Vector2Int, TileBase>();
            foreach (var tile in Tilemap.GetTilesBlock(Tilemap.cellBounds))
            {
                tiles.Add(new Vector2Int(i % width, i / width), tile);
                i++;
            }

            var tilemapOffset = Tilemap.WorldToCell(Vector3.zero);
            var min = Tilemap.cellBounds.min;
            var additionalPos = (Vector2Int)(min - tilemapOffset);

            return tiles
                .Where((t, i) => t.Value != null)
                .Select(t => t.Key + additionalPos)
                .ToArray();
        }
    }
}
