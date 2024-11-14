using System.Collections.Generic;
using System.Linq;
using Anomalus.Items.Owner;
using Anomalus.UIKit.Bars;
using Anomalus.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Anomalus.Items.UI
{
    public abstract class InventoryUI<T> : InventoryUI where T : Inventory
    {
        public T Inventory { get; private set; }
        public override Inventory InventoryBase => Inventory;

        public virtual void MountInventory(T inventory, InventoryMass mass, IInventoryOwner owner)
        {
            if (Inventory != null)
                DismountInventory();

            Inventory = inventory;
            InventoryMass = mass;
            InventoryOwner = owner;

            if (NameText != null)
            {
                UpdateName(Inventory.Name);
                //Inventory.Name.StringChanged += UpdateName;
            }

            RenderSortingMode(GetSortingMode(Inventory.UseSorting, Inventory.IsDescendingSorting));

            if (InventoryMass != null)
                InventoryMass.OnChanged.AddListener(OnMassChanged);

            RefreshMassBar();
        }

        public override void DismountInventory()
        {
            if (NameText != null)
            {
                //Inventory.Name.StringChanged -= UpdateName;
            }

            Inventory = null;
            if (InventoryMass != null)
                InventoryMass.OnChanged.RemoveListener(OnMassChanged);
            InventoryMass = null;
        }

        private void UpdateName(string name)
        {
            NameText.text = name;
        }

        private void OnMassChanged(float mass, float maxMass)
        {
            RefreshMassBar();
        }
    }

    public abstract class InventoryUI : MonoBehaviour, IItemDragTarget
    {
        [SerializeField] private InventoryScreen _screen;

        [SerializeField] protected GameObject CellPrefab;
        [SerializeField] protected Transform CellsHolder;
        [SerializeField] protected RectTransform BoundsRect;
        [SerializeField] protected TMP_Text NameText;
        [SerializeField] protected Bar MassBar;
        [SerializeField] protected bool IsCellsDraggable = true;

        [Header("Sorting")]
        [SerializeField] private Image _sortingModeIcon;
        [SerializeField] private Sprite[] _sortingModeSprites;

        public abstract IEnumerable<InventoryUICell> Cells { get; }
        public abstract Inventory InventoryBase { get; }
        public InventoryMass InventoryMass { get; protected set; }
        public IInventoryOwner InventoryOwner { get; protected set; }
        public InventoryScreen Screen => _screen;

        public enum SortingMode
        {
            Node = 0,
            Descending = 1,
            Ascending = 2
        }

        private SortingMode _currentSortingMode;

        public abstract void DismountInventory();
        public abstract InventoryUICell GetCellAt(int index);

        public void OnClosed()
        {
            foreach (var cell in Cells)
            {
                cell.OnInventoryClosed();
            }
        }

        public bool DropItem(Inventory.Slot stack)
        {
            var targetCell = GetCellUnderMouse();

            if (InventoryBase != stack.Inventory || (targetCell != null && targetCell != this))
            {
                return stack.MoveTo(InventoryBase, targetCell != null ? targetCell.Index : null);
            }

            return false;
        }

        public bool IsMouseOver()
        {
            if (!gameObject.activeInHierarchy) return false;

            if (BoundsRect == null)
            {
                return GetCellUnderMouse() != null;
            }

            return BoundsRect.IsScreenPointOver(Input.mousePosition);
        }

        public InventoryUICell GetCellUnderMouse()
        {
            return Cells.FirstOrDefault(c => c.IsMouseOver());
        }

        public void ToggleSortingMode()
        {
            if (_sortingModeIcon == null) return;

            SetSortingMode((SortingMode)(((int)_currentSortingMode + 1) % 3));
        }

        public void SetSortingMode(SortingMode mode)
        {
            RenderSortingMode(mode);

            var (useSoritng, isDescending) = GetSortingProperties(mode);

            InventoryBase.UseSorting = useSoritng;
            InventoryBase.IsDescendingSorting = isDescending;

            Sort();
        }

        protected void RefreshMassBar()
        {
            if (MassBar == null) return;

            if (InventoryMass != null)
            {
                MassBar.gameObject.SetActive(true);
                MassBar.SetValue(InventoryMass.Mass, InventoryMass.MaxMass);
            }
            else
                MassBar.gameObject.SetActive(false);
        }

        protected abstract void Sort();

        protected void RenderSortingMode(SortingMode mode)
        {
            if (_sortingModeIcon == null) return;

            _currentSortingMode = mode;

            _sortingModeIcon.sprite = _sortingModeSprites[(int)mode];
        }

        protected SortingMode GetSortingMode(bool isActive, bool isDescending)
        {
            return (isActive, isDescending) switch
            {
                (false, _) => SortingMode.Node,
                (true, true) => SortingMode.Descending,
                (true, false) => SortingMode.Ascending
            };
        }

        protected (bool isActive, bool isDescending) GetSortingProperties(SortingMode mode)
        {
            return mode switch
            {
                SortingMode.Node => (false, false),
                SortingMode.Descending => (true, true),
                SortingMode.Ascending => (true, false),
                _ => (false, false)
            };
        }
    }
}
