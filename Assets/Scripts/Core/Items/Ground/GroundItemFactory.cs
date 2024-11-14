using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Items.Ground
{
    public sealed class GroundItemFactory
    {
        private readonly GroundItemConfig _config;
        private readonly IObjectResolver _objectResolver;

        public GroundItemFactory(GroundItemConfig config, IObjectResolver objectResolver)
        {
            _config = config;
            _objectResolver = objectResolver;
        }

        public GroundItem Spawn(ItemStack item, Vector3 position)
        {
            return SpawnSingle(item, position);
        }

        private GroundItem SpawnSingle(ItemStack item, Vector3 position)
        {
            var groundItem = _objectResolver.Instantiate(GetPrefab(item), position, Quaternion.identity)
                .GetComponent<GroundItem>();
            groundItem.Item = item;
            return groundItem;
        }

        private GameObject GetPrefab(ItemStack item)
        {
            return _config.DefaultItemPrefab;
        }
    }
}
