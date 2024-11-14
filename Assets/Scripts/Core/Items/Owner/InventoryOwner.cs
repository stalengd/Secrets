using System.Collections.Generic;
using Anomalus.Items.Configs;
using Anomalus.Items.Ground;
using R3;
using UnityEngine;
using VContainer;

namespace Anomalus.Items.Owner
{
    public sealed class InventoryOwner : IInventoryOwner
    {
        [Inject] private readonly InventoryConfig _config;
        [Inject] private readonly GroundItemFactory _groundItemFactory;
        [Inject] private readonly ItemsUser _itemsUser;

        public Transform ItemsDropPoint { get; set; }

        public GridInventory MainInventory => _mainInventory;
        public Inventory SideInventory => _openedSideInventory;
        public GridInventory QuickInventory => _quickInventory;
        public GridInventory GearInventory => _gearInventory;
        public IEnumerable<Inventory> AllOwnedInventories => _allOwnedInventories;

        public ReadOnlyReactiveProperty<int> SelectedQuickIndex => _selectedQuickIndex;
        public Inventory.Slot SelectedQuickItem => QuickInventory.GetCell(SelectedQuickIndex.CurrentValue);

        private GridInventory _mainInventory;
        private GridInventory _quickInventory;
        private GridInventory _gearInventory;
        private Inventory[] _allOwnedInventories;

        private Inventory _openedSideInventory;
        private readonly ReactiveProperty<int> _selectedQuickIndex = new(0);

        public void Initialize()
        {
            _mainInventory = new GridInventory(_config.MainInventorySize)
            {
                Name = _config.BackpackName,
                UseSorting = true,
                IsDescendingSorting = true
            };
            _quickInventory = new GridInventory(_config.QuickInventorySize, s => s.ItemType.EquipType.HasFlag(ItemEquipType.Main));
            _gearInventory = new GridInventory(_config.GearInventorySize, s => s.ItemType.EquipType.HasFlag(ItemEquipType.Gear));
            _allOwnedInventories = new[] { _quickInventory, _mainInventory, _gearInventory };

            SelectQuickSlot(0);

            _quickInventory.OnSlotChanged.AddListener((s, i) =>
            {
                if (i == SelectedQuickIndex.CurrentValue) EquipItem(s);
            });
            _quickInventory.OnSlotCleared.AddListener((s, i) =>
            {
                if (i == SelectedQuickIndex.CurrentValue) UnequipItem(s);
            });

            foreach (var item in _config.StartItems)
            {
                _allOwnedInventories.AppendInventoryItem(item);
            }
        }

        public void SetSideInventory(Inventory inventory)
        {
            _openedSideInventory = inventory;
        }

        public bool MoveStackToOtherInventory(Inventory.Slot stack, int? count = null)
        {
            var targetInventory = GetOtherInventory(stack.Inventory);
            return stack.MoveTo(targetInventory, null, count);
        }

        public Inventory GetOtherInventory(Inventory inventory)
        {
            if (inventory != MainInventory) return MainInventory;
            return QuickInventory;
        }

        public void DropItem(ItemStack stack)
        {
            // If no point provided - no luck then. Maybe implement some kind of buffer later.
            if (ItemsDropPoint == null)
                return;
            _groundItemFactory.Spawn(stack, ItemsDropPoint.position);
        }

        public void SelectQuickSlot(int index)
        {
            index = Mathf.Clamp(index, 0, _quickInventory.SlotsCount - 1);

            if (SelectedQuickItem != null)
                UnequipItem(SelectedQuickItem);

            _selectedQuickIndex.Value = index;

            if (SelectedQuickItem != null)
                EquipItem(SelectedQuickItem);
            else
                EquipNoItem();
        }

        private void EquipItem(Inventory.Slot item)
        {
            _itemsUser.EquipItem(item.AsStack);
        }

        private void UnequipItem(Inventory.Slot item)
        {
            _itemsUser.UnequipItem(item.AsStack);
        }

        private void EquipNoItem()
        {

        }
    }
}
