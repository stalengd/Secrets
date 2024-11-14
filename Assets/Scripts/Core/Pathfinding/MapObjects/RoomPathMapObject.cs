using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomalus.Pathfinding.MapObjects
{
    public class RoomPathMapObject : TilePathMapObject
    {
        [SerializeField] private Transform _topDoor;
        [SerializeField] private Transform _rightDoor;
        [SerializeField] private Transform _bottomDoor;
        [SerializeField] private Transform _leftDoor;
        [SerializeField] private int _doorHoleWidth = 6;

        private PathMap _map;

        public override Vector2Int[] GetBlockedPoints(PathMap map)
        {
            _map = map;
            var p = base.GetBlockedPoints(map);
            var b = new List<Vector2Int>();
            GetDoorTiles(_topDoor, true, b);
            GetDoorTiles(_rightDoor, false, b);
            GetDoorTiles(_bottomDoor, true, b);
            GetDoorTiles(_leftDoor, false, b);
            return p.Union(b).ToArray();
        }

        private void GetDoorTiles(Transform door, bool isHorizontal, List<Vector2Int> results)
        {
            Vector3 relativePos;
            Vector3 stepOffset;
            if (isHorizontal)
            {
                relativePos = new(-_doorHoleWidth / 2f, 0f);
                stepOffset = new(1f, 0f);
            }
            else
            {
                relativePos = new(0f, -_doorHoleWidth / 2f);
                stepOffset = new(0f, 1f);
            }

            for (var i = 0; i < _doorHoleWidth; i++)
            {
                results.Add((Vector2Int)_map.Grid.WorldToCell(door.position + relativePos));
                relativePos += stepOffset;
            }
        }
    }
}
