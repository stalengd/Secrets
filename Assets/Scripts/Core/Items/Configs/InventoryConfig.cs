using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Anomalus.Items.Configs
{
    [CreateAssetMenu(menuName = "Game Data/Items/Inventory Config")]
    public sealed class InventoryConfig : ScriptableObject
    {
        [SerializeField] private List<ItemStack> _startItems;

        [Space]
        [SerializeField] private int _mainInventorySize = 40;
        [SerializeField] private int _quickInventorySize = 10;
        [SerializeField] private int _gearInventorySize = 1;

        [Space]
        [SerializeField] private string _backpackName;

        [Space]
        [SerializeField] private InputActionReference _openInventoryInput;
        [SerializeField] private InputActionReference _useQuickSlotInput;
        [SerializeField] private InputActionReference _scrollSlotsInput;
        [SerializeField] private InputActionReference[] _useGearSlotsInput;

        public IReadOnlyList<ItemStack> StartItems => _startItems;

        public int MainInventorySize => _mainInventorySize;
        public int QuickInventorySize => _quickInventorySize;
        public int GearInventorySize => _gearInventorySize;

        public string BackpackName => _backpackName;

        public InputActionReference OpenInventoryInput => _openInventoryInput;
        public InputActionReference UseQuickSlotInput => _useQuickSlotInput;
        public InputActionReference ScrollSlotsInput => _scrollSlotsInput;
        public IReadOnlyList<InputActionReference> UseGearSlotsInput => _useGearSlotsInput;
    }
}
