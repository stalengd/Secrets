using Anomalus.Items.Owner;
using Anomalus.Items.UI;
using Anomalus.Pathfinding;
using Anomalus.Player;
using Anomalus.Rendering;
using Anomalus.Scientists;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
    public sealed class MainFlow : IStartable, ITickable
    {
        [Inject] private readonly PlayerSpawner _playerSpawner;
        [Inject] private readonly ScientistsFlow _scientistsFlow;
        [Inject] private readonly InventoryControl _inventoryControl;
        [Inject] private readonly IInventoryOwner _inventoryOwner;
        [Inject] private readonly InventoryScreen _inventoryScreen;
        [Inject] private readonly Pathfinder _pathfinder;
        [Inject] private readonly CameraController _cameraController;

        public void Start()
        {
            InitializePlayer();
            _pathfinder.Map.GenerateMap();
            _scientistsFlow.Start();
        }

        public void Tick()
        {
            _inventoryControl.UpdateInput();
            _pathfinder.Update();
        }

        private void InitializePlayer()
        {
            _inventoryOwner.Initialize();
            _inventoryScreen.Initialize();
            var player = _playerSpawner.Spawn();
            _inventoryOwner.ItemsDropPoint = player.transform;
            _cameraController.Track(player.transform);
        }
    }
}
