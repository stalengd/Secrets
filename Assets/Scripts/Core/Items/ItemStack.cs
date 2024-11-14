using UnityEngine;

namespace Anomalus.Items
{
    [System.Serializable]
    public struct ItemStack
    {
        [SerializeField] private ItemConfig _itemType;
        [SerializeField] private int _count;
        [SerializeField] private ItemAttributes _attributes;

        public readonly ItemConfig ItemType => _itemType;
        public readonly int Count => _count;
        public readonly ItemAttributes Attributes => _attributes;

        public ItemStack(ItemConfig itemType, int count, ItemAttributes attributes)
        {
            _itemType = itemType;
            _count = count;
            _attributes = attributes;
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
        //        ["attributes"] = attributes.ToJson(), 
        //    };
        //}

        //public static ItemStack FromJson(JToken json)
        //{
        //    return new ItemStack(
        //        itemType: ItemConfig.Get(json["id"].Value<string>()), 
        //        count: json["count"].Value<int>(),
        //        attributes: ItemAttributes.FromJson(json["attributes"])
        //        );
        //}
    }

    public struct ItemAttributes
    {
        public double LastUseTime;
    }
}
