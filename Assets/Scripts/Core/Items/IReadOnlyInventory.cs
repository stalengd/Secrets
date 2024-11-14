using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Anomalus.Items
{
    public interface IReadOnlyInventory : IItemsAvailabilityChecker
    {
        Inventory.Slot this[int index] { get; }

        Predicate<ItemStack> Filter { get; }
        bool IsDescendingSorting { get; }
        bool IsSlotsLimited { get; }
        IEnumerable<Inventory.Slot> Slots { get; }
        InventoryMass Mass { get; }
        string Name { get; }
        UnityEvent OnChange { get; }
        UnityEvent<Inventory.Slot> OnItemAdded { get; }
        UnityEvent<Inventory.Slot> OnItemChanged { get; }
        UnityEvent<Inventory.Slot> OnItemRemoved { get; }
        int SlotsCount { get; }
        Func<ItemConfig, float> SortingSelector { get; }
        bool UseSorting { get; }

        Inventory.Slot GetCell(int index);
        Inventory.Slot GetStackWithItem(ItemConfig itemData, int skip = 0);
        int GetStackIndexWithItem(ItemConfig itemData, int skip = 0);
    }
}
