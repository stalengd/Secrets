using Anomalus.AI;
using Anomalus.Player;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
	public sealed class MainFlow : IStartable
	{
		[Inject] private readonly PlayerSpawner _playerSpawner;
		[Inject] private readonly SpawnPresetCollection _spawnPresetCollection;
		
		public void Start()
		{
			_playerSpawner.Spawn();
			
			foreach(var spawnPreset in _spawnPresetCollection.SpawnPresets)
				if (spawnPreset.SpawnOnStart)
					_spawnPresetCollection.Spawn(spawnPreset);
		}
	}
}
