using System.Collections.Generic;
using Anomalus.Items.Owner;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Items.UI
{
    public class GridInventoryUI : InventoryUI<GridInventory>
    {
        [Inject] private readonly IObjectResolver _objectResolver;

        public override IEnumerable<InventoryUICell> Cells => _cells;

        private readonly List<InventoryUICell> _cells = new();

        public override void MountInventory(GridInventory inventory, InventoryMass mass, IInventoryOwner owner)
        {
            base.MountInventory(inventory, mass, owner);

            foreach (var item in inventory.Slots)
            {
                var slot = AddSlot();

                slot.SetState(item);
            }

            inventory.OnSlotChanged.AddListener(OnSlotChanged);
            inventory.OnSlotCleared.AddListener(OnSlotCleared);
        }

        public override void DismountInventory()
        {
            Inventory.OnSlotChanged.RemoveListener(OnSlotChanged);
            Inventory.OnSlotCleared.RemoveListener(OnSlotCleared);

            foreach (var cell in _cells)
            {
                if (cell == null) continue;
                Destroy(cell.gameObject);
            }
            _cells.Clear();

            base.DismountInventory();
        }

        public override InventoryUICell GetCellAt(int index)
        {
            return index >= 0 && index < _cells.Count ? _cells[index] : null;
        }

        private InventoryUICell AddSlot()
        {
            var cell = _objectResolver.Instantiate(CellPrefab, CellsHolder).GetComponent<InventoryUICell>();
            cell.IsDraggable = IsCellsDraggable;
            cell.Index = _cells.Count;
            cell.Inventory = this;
            _cells.Add(cell);
            return cell;
        }

        private void OnSlotChanged(Inventory.Slot item, int index)
        {
            _cells[index].SetState(item);
        }

        private void OnSlotCleared(Inventory.Slot item, int index)
        {
            var cell = _cells[index];
            cell.SetState(null);
        }

        protected override void Sort()
        {

        }
    }
}
