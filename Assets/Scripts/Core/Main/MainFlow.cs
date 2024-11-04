using Anomalus.Player;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
    public sealed class MainFlow : IStartable
    {
        [Inject] private readonly PlayerSpawner _playerSpawner;

        public void Start()
        {
            _playerSpawner.Spawn();
        }
    }
}
