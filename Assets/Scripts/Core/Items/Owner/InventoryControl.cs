using Anomalus.Items.Configs;
using Anomalus.Items.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Anomalus.Items.Owner
{
    public sealed class InventoryControl
    {
        [Inject] private readonly InventoryScreen _inventoryScreen;
        [Inject] private readonly IInventoryOwner _inventoryOwner;
        [Inject] private readonly InventoryConfig _inventoryConfig;
        [Inject] private readonly ItemsUser _itemsUser;
        [Inject] private readonly ItemSystemManager _itemSystemManager;

        public void UpdateInput()
        {
            HandleToggleInventory();
            HandleQuickSlotsSelection();
            HandleUseQuickSelectionItem();
            HandleUseGearItem();
        }

        private void HandleToggleInventory()
        {
            if (_inventoryConfig.OpenInventoryInput.action.WasPressedThisFrame())
            {
                _inventoryScreen.SetShowState(!_inventoryScreen.InventoryShown);
            }
        }

        private void HandleQuickSlotsSelection()
        {
            var pressedNumber = GetNumberInput();
            if (pressedNumber.HasValue)
            {
                _inventoryOwner.SelectQuickSlot(pressedNumber.Value - 1);
            }

            var scrollSlots = _inventoryConfig.ScrollSlotsInput.action.ReadValue<Vector2>();
            if (scrollSlots.y > 0f)
                ScrollQuickSlotsTo(1);
            else if (scrollSlots.y < 0f)
                ScrollQuickSlotsTo(-1);
        }

        private void HandleUseQuickSelectionItem()
        {
            var slot = _inventoryOwner.SelectedQuickItem;
            if (_inventoryConfig.UseQuickSlotInput.action.WasPressedThisFrame()
                && slot != null
                && _itemSystemManager.Get(slot.Item) is { IsUsable: true } system)
            {
                system.UseItem(slot, _itemsUser);
            }
        }

        private void HandleUseGearItem()
        {
            for (var i = 0; i < _inventoryConfig.UseGearSlotsInput.Count; i++)
            {
                var input = _inventoryConfig.UseGearSlotsInput[i];
                var slot = _inventoryOwner.GearInventory[i];
                if (input.action.WasPressedThisFrame()
                    && slot != null
                    && _itemSystemManager.Get(slot.Item) is { IsUsable: true } system)
                {
                    system.UseItem(slot, _itemsUser);
                }
            }
        }

        private void ScrollQuickSlotsTo(int dir)
        {
            var slot = _inventoryOwner.SelectedQuickIndex.CurrentValue;
            slot += dir;
            if (slot < 0)
                slot += _inventoryOwner.QuickInventory.SlotsCount;
            else
                slot %= _inventoryOwner.QuickInventory.SlotsCount;
            _inventoryOwner.SelectQuickSlot(slot);
        }

        private int? GetNumberInput()
        {
            for (var i = 0; i < 9; i++)
            {
                if (Keyboard.current[Key.Digit1 + i].isPressed)
                    return i + 1;
            }
            return null;
        }
    }
}
