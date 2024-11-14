using UnityEngine;

namespace Anomalus.Items
{
    [CreateAssetMenu(menuName = "Game Data/Items/Types/Basic Item")]
    public class ItemConfig : ScriptableObject
    {
        public string Id => name;

        [SerializeField] private ItemTypeSpecialization _specialization;
        public ItemTypeSpecialization Specialization => _specialization;

        [SerializeField] private Sprite _sprite;
        public Sprite Sprite => _sprite;


        [Header("UI")]
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private string _description;
        public string Description => _description;


        [Header("Inventory Behavior")]
        [SerializeField] private float _mass = 1f;
        public float Mass => _mass;

        [SerializeField] private ItemEquipType _equipType;
        public ItemEquipType EquipType => _equipType;

        [SerializeField] private int _maxStack = -1;
        public int MaxStack => _maxStack;

        [SerializeField] private int _sortingOrder = 0;
        public int SortingOrder => _sortingOrder;

        [Header("Shop Behavior")]
        [SerializeField] private int _baseShopPrice = 1;
        public int BaseShopPrice => _baseShopPrice;

        [SerializeField] private bool _canBeSold = true;
        public bool CanBeSold => _canBeSold;

        public override string ToString()
        {
            return $"'{Name}' [{Id}]";
        }
    }

    [System.Flags]
    public enum ItemEquipType
    {
        None = 0,
        Main = 1,
        Gear = 2
    }
}
