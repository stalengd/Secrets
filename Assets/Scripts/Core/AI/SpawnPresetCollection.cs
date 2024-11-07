using System.Collections.Generic;
using UnityEngine;

namespace Anomalus.AI
{
	public sealed class SpawnPresetCollection : MonoBehaviour
	{
		[SerializeField] private List<SpawnPreset> _spawnPresets;
		
		public List<SpawnPreset> SpawnPresets => _spawnPresets;
	}
}