using UnityEngine;

namespace Anomalus.Items.Ground
{
    [CreateAssetMenu(menuName = "Game Data/Items/Ground Item Config")]
    public sealed class GroundItemConfig : ScriptableObject
    {
        [SerializeField] private GameObject _defaultItemPrefab;

        public GameObject DefaultItemPrefab => _defaultItemPrefab;
    }
}
