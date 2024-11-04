using Secrets.Player;
using VContainer;
using VContainer.Unity;

namespace Secrets.Main
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
