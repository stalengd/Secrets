using UnityEngine;

namespace Anomalus.AI
{
	public sealed class SpawnPreset : MonoBehaviour
	{
		[SerializeField] private GameObject _prefab;
		//[SerializeField] private bool _spawnOnStart;
		
		public GameObject Prefab => _prefab;
		
		public Vector2 Position => transform.position;
	}
}