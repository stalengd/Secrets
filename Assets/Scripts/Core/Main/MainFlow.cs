using Anomalus.AI;
using Anomalus.Player;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
	public sealed class MainFlow : IStartable
	{
		[Inject] private readonly PlayerSpawner _playerSpawner;
		[Inject] private readonly AIFactory _aiFactory;
		[Inject] private readonly SpawnPresetCollection _spawnPresetCollection;
		
		public void Start()
		{
			_playerSpawner.Spawn();
			
			foreach(var spawnerPreset in _spawnPresetCollection.SpawnPresets)
				_aiFactory.Create(spawnerPreset.Prefab, spawnerPreset.Position);
		}
	}
}
