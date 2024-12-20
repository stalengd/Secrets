using Anomalus.AI;
using Anomalus.Infrastructure.Configurators;
using Anomalus.Items;
using Anomalus.Items.Ground;
using Anomalus.Items.Owner;
using Anomalus.Pathfinding;
using Anomalus.Player;
using Anomalus.Rendering;
using Anomalus.Scientists;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
    public sealed class MainLifetimeScope : LifetimeScope
    {
        [SerializeField] private MonoConfigurator[] _monoConfigurators;

        [Space]
        [SerializeField] private PlayerSpawner _playerSpawner;

        [Space]
        [SerializeField] private ScientistSpawnPresetCollection _scientistSpawnPresetCollection;
        [SerializeField] private PathMap _pathMap;
        [SerializeField] private CameraController _cameraController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainFlow>(Lifetime.Singleton);

            foreach (var configurator in _monoConfigurators)
            {
                configurator.Configure(builder);
            }

            builder.RegisterComponent(_playerSpawner);

            // Register factory for AI spawners && register the spawners
            builder.Register<AIFactory>(Lifetime.Singleton);
            builder.Register<ScientistsFlow>(Lifetime.Singleton);
            builder.RegisterComponent(_scientistSpawnPresetCollection);

            builder.RegisterComponent(_pathMap);
            builder.Register<Pathfinder>(Lifetime.Singleton);

            builder.RegisterComponent(_cameraController);

            ConfigureItems(builder);
        }

        private void ConfigureItems(IContainerBuilder builder)
        {
            builder.Register<IInventoryOwner, InventoryOwner>(Lifetime.Singleton);
            builder.Register<ItemsUser>(Lifetime.Singleton);
            builder.Register<GroundItemFactory>(Lifetime.Singleton);
            builder.Register<GroundItemsRegistry>(Lifetime.Singleton);
            builder.Register<InventoryControl>(Lifetime.Singleton);
            builder.Register<ItemSystemManager>(Lifetime.Singleton);
        }
    }
}
