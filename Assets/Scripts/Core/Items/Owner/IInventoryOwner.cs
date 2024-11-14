using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Anomalus.Items.Owner
{
    public interface IInventoryOwner
    {
        Transform ItemsDropPoint { get; set; }
        GridInventory MainInventory { get; }
        Inventory SideInventory { get; }
        GridInventory QuickInventory { get; }
        GridInventory GearInventory { get; }
        IEnumerable<Inventory> AllOwnedInventories { get; }
        Inventory.Slot SelectedQuickItem { get; }
        ReadOnlyReactiveProperty<int> SelectedQuickIndex { get; }

        void Initialize();
        void SetSideInventory(Inventory inventory);
        void DropItem(ItemStack bunch);
        Inventory GetOtherInventory(Inventory inventory);
        bool MoveStackToOtherInventory(Inventory.Slot stack, int? count = null);
        void SelectQuickSlot(int index);
    }
}
