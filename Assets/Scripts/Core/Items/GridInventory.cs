using UnityEngine.Events;

namespace Anomalus.Items
{
    public class GridInventory : Inventory
    {
        public UnityEvent<Slot, int> OnSlotChanged { get; } = new UnityEvent<Slot, int>();
        public UnityEvent<Slot, int> OnSlotCleared { get; } = new UnityEvent<Slot, int>();

        public GridInventory(int slots, System.Predicate<ItemStack> filter = null) : base(slots, filter)
        {
            InsertItemsAsList = false;

            OnItemChanged.AddListener(i => OnSlotChanged.Invoke(i, Items.IndexOf(i)));

            for (var i = 0; i < slots; i++)
            {
                Items.Add(null);
            }
        }

        public override int CanAddItem(ItemStack stack, int? slot, Inventory from = null)
        {
            if (slot.HasValue && slot.Value >= SlotsCount)
                return 0;

            var baseResult = base.CanAddItem(stack, slot, from);
            if (slot.HasValue)
                return baseResult;
            if (baseResult == 0)
                return 0;

            int addToNewStack = 0;
            int addToOldStack = 0;

            foreach (var (_, _, sumAdded) in GetStacksForAdding(stack))
            {
                addToOldStack = sumAdded;
            }
            if (HasEmptySlot())
            {
                addToNewStack = baseResult - addToOldStack;
            }

            return addToNewStack + addToOldStack;
        }

        public override void RemoveItem(Slot item)
        {
            var slot = Items.IndexOf(item);
            Items[slot] = null;

            if (item.Inventory == this)
                item.IsRemoved = true;
            OnItemRemoved.Invoke(item);
            OnSlotCleared.Invoke(item, slot);
        }

        protected override Slot AddItem(Slot item, int? slot)
        {
            if (!slot.HasValue)
            {
                slot = Items.FindIndex(c => c == null);
                if (slot.Value < 0)
                    throw new System.InvalidOperationException($"Item can not be added, there are no empty slots while no slot specified");
            }

            if (slot.Value >= SlotsCount)
            {
                throw new System.InvalidOperationException($"Item can not be added to slot {slot.Value} while there are only {SlotsCount} slots.");
            }

            var prevItem = Items[slot.Value];
            if (prevItem != null)
                RemoveItem(prevItem);
            Items[slot.Value] = item;

            OnItemAdded.Invoke(item);
            OnItemChanged.Invoke(item);
            OnSlotChanged.Invoke(item, slot.Value);

            return item;
        }
    }
}
