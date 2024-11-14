using Anomalus.AI;
using Anomalus.Items.Owner;
using Anomalus.Items.UI;
using Anomalus.Player;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
    public sealed class MainFlow : IStartable, ITickable
    {
        [Inject] private readonly PlayerSpawner _playerSpawner;
        [Inject] private readonly AIFactory _aiFactory;
        [Inject] private readonly SpawnPresetCollection _spawnPresetCollection;
        [Inject] private readonly InventoryControl _inventoryControl;
        [Inject] private readonly IInventoryOwner _inventoryOwner;
        [Inject] private readonly InventoryScreen _inventoryScreen;

        public void Start()
        {
            InitializePlayer();

            foreach (var spawnerPreset in _spawnPresetCollection.SpawnPresets)
            {
                _aiFactory.Create(spawnerPreset.Prefab, spawnerPreset.Position);
            }
        }

        public void Tick()
        {
            _inventoryControl.UpdateInput();
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
