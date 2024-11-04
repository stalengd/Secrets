using Secrets.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Secrets.Main
{
    public class MainLifetimeScope : LifetimeScope
    {
        [Space]
        [SerializeField] private PlayerSpawner _playerSpawner;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainFlow>(Lifetime.Singleton);

            builder.RegisterComponent(_playerSpawner);

        }
    }
}
