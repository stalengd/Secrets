using System.Collections.Generic;
using UnityEngine;

namespace Anomalus.Items.Configs
{
    [CreateAssetMenu(menuName = "Game Data/Items/Items Database")]
    public sealed class ItemsDatabase : ScriptableObject
    {
        private readonly Dictionary<string, ItemConfig> _itemsData = new();

        private void Awake()
        {
            foreach (var itemData in Resources.FindObjectsOfTypeAll<ItemConfig>())
            {
                _itemsData.Add(itemData.Id, itemData);
            }
        }

        public ItemConfig GetOrDefault(string itemId)
        {
            return _itemsData.GetValueOrDefault(itemId);
        }
    }
}
