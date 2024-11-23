using System.Collections.Generic;
using UnityEngine;

namespace Anomalus.Scientists
{
    public sealed class ScientistSpawnPresetCollection : MonoBehaviour
    {
        public List<ScientistSpawnPreset> SpawnPresets => _spawnPresets;

        private readonly List<ScientistSpawnPreset> _spawnPresets = new();

        private void Awake()
        {
            GetComponentsInChildren(_spawnPresets);
        }
    }
}
