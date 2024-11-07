using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.AI
{
	public sealed class AIFactory
	{
		[Inject] private IObjectResolver _resolver;
		
		public GameObject Create(GameObject prefab, Vector2 position) => _resolver.Instantiate(prefab, position, Quaternion.identity);
	}
}
