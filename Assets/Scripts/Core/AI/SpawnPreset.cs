using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomalus.AI
{
	public sealed class SpawnPreset : MonoBehaviour
	{
		[SerializeField] private GameObject _prefab;
		[SerializeField] private bool _spawnOnStart;
		[SerializeField] private List<Waypoint> _movementWaypoints;
		
		public GameObject Prefab => _prefab;
		
		public Vector2 Position => transform.position;
		
		public bool SpawnOnStart => _spawnOnStart;
		
		public List<Waypoint> MovementWaypoints => _movementWaypoints;
		
		private void OnDrawGizmosSelected()
		{	
			for (int i = 0; i < _movementWaypoints.Count; i++)
			{
				Gizmos.DrawSphere(_movementWaypoints[i].Position, 0.2f);
				
				if (i < _movementWaypoints.Count - 1)
					Gizmos.DrawLine(_movementWaypoints[i].Position, _movementWaypoints[i+1].Position);
				else
					Gizmos.DrawLine(_movementWaypoints[i].Position, _movementWaypoints.First().Position);
			}
		}
	}
}