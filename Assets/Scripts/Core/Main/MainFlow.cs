using Anomalus.AI;
using Anomalus.Items.Owner;
using Anomalus.Items.UI;
using Anomalus.Pathfinding;
using Anomalus.Player;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
    public sealed class MainFlow : IStartable, ITickable
    {
        [Inject] private readonly PlayerSpawner _playerSpawner;
        [Inject] private readonly SpawnPresetCollection _spawnPresetCollection;
        [Inject] private readonly InventoryControl _inventoryControl;
        [Inject] private readonly IInventoryOwner _inventoryOwner;
        [Inject] private readonly InventoryScreen _inventoryScreen;
        [Inject] private readonly Pathfinder _pathfinder;

        public void Start()
        {
            InitializePlayer();

            foreach (var spawnPreset in _spawnPresetCollection.SpawnPresets)
                if (spawnPreset.SpawnOnStart)
                    _spawnPresetCollection.Spawn(spawnPreset);

            _pathfinder.Map.GenerateMap();
        }

        public void Tick()
        {
            _inventoryControl.UpdateInput();
            _pathfinder.Update();
        }

        public void InitializePlayer()
        {
            _inventoryOwner.Initialize();
            _inventoryScreen.Initialize();
            var player = _playerSpawner.Spawn();
            _inventoryOwner.ItemsDropPoint = player.transform;
        }
    }
}
