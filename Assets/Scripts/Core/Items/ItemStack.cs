using UnityEngine;

namespace Anomalus.Items
{
    [System.Serializable]
    public struct ItemStack
    {
        [SerializeField] private ItemConfig _itemType;
        [SerializeField] private int _count;
        [SerializeField] private ItemExtraState _state;

        public readonly ItemConfig ItemType => _itemType;
        public readonly int Count => _count;
        public readonly ItemExtraState State => _state;

        public ItemStack(ItemConfig itemType, int count, ItemExtraState state)
        {
            _itemType = itemType;
            _count = count;
            _state = state;
        }

        public override readonly string ToString()
        {
            return $"{ItemType} x{Count}";
        }

        //public JObject ToJson()
        //{
        //    return new JObject()
        //    {
        //        ["id"] = ItemType.Id, 
        //        ["count"] = Count, 
        //        ["state"] = state.ToJson(), 
        //    };
        //}

        //public static ItemStack FromJson(JToken json)
        //{
        //    return new ItemStack(
        //        itemType: ItemConfig.Get(json["id"].Value<string>()), 
        //        count: json["count"].Value<int>(),
        //        state: ItemExtraState.FromJson(json["state"])
        //        );
        //}
    }

    public struct ItemExtraState
    {
        public double LastUseTime;
    }
}
