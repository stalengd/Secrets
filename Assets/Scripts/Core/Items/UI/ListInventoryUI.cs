using System.Collections.Generic;
using System.Linq;
using Anomalus.Items.Owner;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Items.UI
{
    public class ListInventoryUI : InventoryUI<Inventory>
    {
        [Inject] private readonly IObjectResolver _objectResolver;

        public override IEnumerable<InventoryUICell> Cells => _cells.Values;

        private readonly Dictionary<Inventory.Slot, InventoryUICell> _cells = new();

        public override void MountInventory(Inventory inventory, InventoryMass mass, IInventoryOwner owner)
        {
            base.MountInventory(inventory, mass, owner);

            foreach (var item in inventory.Slots)
            {
                AddItemInternal(item);
            }

            Inventory.OnItemAdded.AddListener(AddItem);
            Inventory.OnItemChanged.AddListener(UpdateItem);
            Inventory.OnItemRemoved.AddListener(DestroyCell);

            Sort();
        }

        public override void DismountInventory()
        {
            Inventory.OnItemAdded.RemoveListener(AddItem);
            Inventory.OnItemChanged.RemoveListener(UpdateItem);
            Inventory.OnItemRemoved.RemoveListener(DestroyCell);

            foreach (var cell in _cells)
            {
                if (cell.Value == null) continue;
                Destroy(cell.Value.gameObject);
            }
            _cells.Clear();

            base.DismountInventory();
        }

        public override InventoryUICell GetCellAt(int index)
        {
            return _cells.ElementAtOrDefault(index).Value; // Questionable implementation
        }

        protected void AddItem(Inventory.Slot item)
        {
            AddItemInternal(item);

            Sort();
        }

        protected void AddItemInternal(Inventory.Slot item)
        {
            var cell = _objectResolver.Instantiate(CellPrefab, CellsHolder).GetComponent<InventoryUICell>();
            cell.IsDraggable = IsCellsDraggable;
            _cells.Add(item, cell);
            _cells[item].SetState(item);
        }

        protected void UpdateItem(Inventory.Slot item)
        {
            _cells[item].SetState(item);
        }

        protected void DestroyCell(Inventory.Slot item)
        {
            Destroy(_cells[item].gameObject);
            _cells.Remove(item);
        }

        protected override void Sort()
        {
            if (!Inventory.UseSorting) return;

            var ordered = Inventory.IsDescendingSorting ?
                _cells.OrderByDescending(c => Inventory.SortingSelector(c.Key.Item)) :
                _cells.OrderBy(c => Inventory.SortingSelector(c.Key.Item));

            var i = 0;
            foreach (var cell in ordered)
            {
                cell.Value.RectTransform.SetSiblingIndex(i);
                i++;
            }
        }
    }
}
