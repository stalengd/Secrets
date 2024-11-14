namespace Anomalus.Items
{
    public readonly struct FullItemAddress
    {
        public Inventory Inventory { get; }
        public int? Slot { get; }

        public FullItemAddress(Inventory inventory, int? slot)
        {
            Inventory = inventory;
            Slot = slot;
        }

        public (Inventory.Slot insertedItem, ItemStack? removedItem) PlaceItem(ItemStack itemStack)
        {
            if (Slot.HasValue)
                return Inventory.InsertItemToSlot(itemStack, Slot.Value);
            else
                return (Inventory.AppendItem(itemStack, false), null);
        }

        public override string ToString()
        {
            return $"{{{Inventory?.Name}, {Slot}}}";
        }
    }
}
