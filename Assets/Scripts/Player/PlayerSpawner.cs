using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Secrets.Player
{
    public sealed class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        [Inject] private readonly IObjectResolver _resolver;

        public void Spawn()
        {
            _resolver.Instantiate(_prefab, transform.position, Quaternion.identity);
        }
    }
}
