using Anomalus.AI;
using VContainer;

namespace Anomalus.Scientists
{
    public sealed class ScientistsFlow
    {
        [Inject] private readonly ScientistSpawnPresetCollection _spawnPresetCollection;
        [Inject] private readonly AIFactory _aiFactory;

        public void Start()
        {
            SpawnStart();
        }

        private void SpawnStart()
        {
            foreach (var preset in _spawnPresetCollection.SpawnPresets)
            {
                SpawnFromPreset(preset);
            }
        }

        private void SpawnFromPreset(ScientistSpawnPreset spawnPreset)
        {
            var agent = _aiFactory.Create(spawnPreset.Prefab, spawnPreset.Position);

            // Set movement path and start moving after spawning
            if (agent.TryGetComponent<WaypointMovement>(out var waypointMovement))
                waypointMovement.TrySetPath(spawnPreset.MovementWaypoints, true);
        }
    }
}
