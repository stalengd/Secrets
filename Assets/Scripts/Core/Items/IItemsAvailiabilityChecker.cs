namespace Anomalus.Items
{
    public interface IItemsAvailabilityChecker
    {
        bool HasItem(ItemStack stack);
        int GetItemCount(ItemConfig itemType);
    }
}
