using Anomalus.AI;
using Anomalus.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
	public class MainLifetimeScope : LifetimeScope
	{
		[Space]
		[SerializeField] private PlayerSpawner _playerSpawner;
		
		[Space]
		[SerializeField] private SpawnPresetCollection _spawnPresetCollection;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<MainFlow>(Lifetime.Singleton);

			builder.RegisterComponent(_playerSpawner);
			
			// Register factory for AI spawners && register the spawners
			builder.Register<AIFactory>(Lifetime.Singleton);
			builder.RegisterComponent(_spawnPresetCollection);
		}
	}
}
