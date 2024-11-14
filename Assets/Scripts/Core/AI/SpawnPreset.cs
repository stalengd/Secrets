using System.Collections.Generic;
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
	}
}