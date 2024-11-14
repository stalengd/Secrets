using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Anomalus.AI
{
	public sealed class SpawnPresetCollection : MonoBehaviour
	{
		[SerializeField] private List<SpawnPreset> _spawnPresets;
		
		[Inject] private readonly AIFactory _aiFactory;
		
		public List<SpawnPreset> SpawnPresets => _spawnPresets;
		
		public void Spawn(SpawnPreset spawnPreset)
		{
			var agent = _aiFactory.Create(spawnPreset.Prefab, spawnPreset.Position);
			
			// Set movement path and start moving after spawning
			if (agent.TryGetComponent<WaypointMovement>(out var waypointMovement))
				waypointMovement.TrySetPath(spawnPreset.MovementWaypoints, true);
		}
	}
}