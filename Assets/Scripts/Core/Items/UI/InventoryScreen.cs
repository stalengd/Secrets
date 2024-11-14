using Anomalus.Items.Owner;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Items.UI
{
    public sealed class InventoryScreen : MonoBehaviour
    {
        [SerializeField] private GridInventoryUI _mainInventoryUi;
        [SerializeField] private GridInventoryUI _otherInventoryUi;
        [SerializeField] private GridInventoryUI _quickInventoryUi;
        [SerializeField] private GridInventoryUI _gearInventoryUi;
        [SerializeField] private CraftUI _craftUi;
        [SerializeField] private RectTransform _topLayer;

        [field: Space]
        [field: SerializeField] public InputActionReference MoveStackInput { get; private set; }
        [field: SerializeField] public InputActionReference SplitStackInput { get; private set; }

        [Inject] private readonly IInventoryOwner _inventoryOwner;
        [Inject] private readonly IObjectResolver _objectResolver;

        public bool InventoryShown => _mainInventoryUi.gameObject.activeSelf;
        public bool IsItemsInteractable => InventoryShown;

        public void Initialize()
        {
            _mainInventoryUi.MountInventory(_inventoryOwner.MainInventory, null, _inventoryOwner);
            _quickInventoryUi.MountInventory(_inventoryOwner.QuickInventory, null, _inventoryOwner);
            _gearInventoryUi.MountInventory(_inventoryOwner.GearInventory, null, _inventoryOwner);
            _inventoryOwner.SelectedQuickIndex.Pairwise().Subscribe(x =>
            {
                SetQuickCellSelected(x.Previous, false);
                SetQuickCellSelected(x.Current, true);
            }).AddTo(this);
            SetQuickCellSelected(_inventoryOwner.SelectedQuickIndex.CurrentValue, true);
        }

        public IItemDragTarget GetInventoryOverMouse()
        {
            if (_mainInventoryUi.IsMouseOver()) return _mainInventoryUi;
            if (_otherInventoryUi.IsMouseOver()) return _otherInventoryUi;
            if (_quickInventoryUi.IsMouseOver()) return _quickInventoryUi;
            if (_gearInventoryUi.IsMouseOver()) return _gearInventoryUi;
            return null;
        }

        public ItemRenderer CreateStackCopyOnTopLayer(InventoryUICell original, GameObject prefab)
        {
            var r = _objectResolver.Instantiate(prefab, _topLayer).GetComponent<ItemRenderer>();
            r.Render(original.Stack.AsStack);
            return r;
        }

        public bool TryOpenMainInventory()
        {
            gameObject.SetActive(true);
            return true;
        }

        public void CloseMainInventory()
        {
            gameObject.SetActive(false);
        }

        public void OpenSideInventory(GridInventory inventory, InventoryMass mass)
        {
            TryOpenMainInventory();

            _otherInventoryUi.gameObject.SetActive(true);
            _otherInventoryUi.MountInventory(inventory, mass, _inventoryOwner);
        }

        public void CloseSideInventory()
        {
            CloseMainInventory();

            _otherInventoryUi.DismountInventory();
            if (_otherInventoryUi != null)
                _otherInventoryUi.gameObject.SetActive(false);
        }

        public void OpenCraft(Crafter crafter)
        {
            TryOpenMainInventory();

            _craftUi.gameObject.SetActive(true);
            _craftUi.Render(crafter, _inventoryOwner.AllOwnedInventories);
        }

        public void CloseCraft()
        {
            CloseMainInventory();

            _craftUi.gameObject.SetActive(false);
        }

        public void SetShowState(bool show)
        {
            if (show)
            {
                _mainInventoryUi.gameObject.SetActive(true);
            }
            else
            {
                _quickInventoryUi.OnClosed();
                _mainInventoryUi.OnClosed();
                _gearInventoryUi.OnClosed();

                _mainInventoryUi.gameObject.SetActive(false);
                _otherInventoryUi.gameObject.SetActive(false);
                _craftUi.gameObject.SetActive(false);

                foreach (Transform t in _topLayer)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        private void SetQuickCellSelected(int index, bool isSelected)
        {
            if (_quickInventoryUi.GetCellAt(index) is InventoryUICellSelectable selectableCell)
            {
                selectableCell.IsSelected = isSelected;
            }
        }
    }
}
