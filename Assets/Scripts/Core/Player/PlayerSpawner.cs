using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Player
{
    public sealed class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        [Inject] private readonly IObjectResolver _resolver;

        public GameObject Instance { get; private set; }

        public GameObject Spawn()
        {
            if (Instance == null)
            {
                Instance = _resolver.Instantiate(_prefab, transform.position, Quaternion.identity);
            }
            return Instance;
        }
    }
}
