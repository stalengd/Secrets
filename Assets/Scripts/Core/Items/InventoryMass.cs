using UnityEngine.Events;

namespace Anomalus.Items
{
    public class InventoryMass
    {
        public float Mass
        {
            get => _mass;
            private set => _mass = value;
        }
        public float MaxMass { get; set; }

        public UnityEvent<float, float> OnChanged = new();

        private float _mass;

        private readonly Inventory[] _inventories;

        public InventoryMass(float maxMass, params Inventory[] inventories)
        {
            MaxMass = maxMass;
            _inventories = inventories;

            foreach (var inventory in inventories)
            {
                inventory.OnChange.AddListener(RecalculateMass);
            }

            RecalculateMass();
        }

        public void RecalculateMass()
        {
            var result = 0f;

            foreach (var inventory in _inventories)
            {
                foreach (var item in inventory.Slots)
                {
                    if (item == null || item.IsRemoved) continue;

                    result += item.Count * item.Item.Mass;
                }
            }

            Mass = result;
            OnChanged.Invoke(Mass, MaxMass);
        }
    }
}
