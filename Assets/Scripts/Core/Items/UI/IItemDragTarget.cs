namespace Anomalus.Items.UI
{
    public interface IItemDragTarget
    {
        bool DropItem(Inventory.Slot stack);
        bool IsMouseOver();
    }
}
