using System.Collections.Generic;
using System.Linq;

namespace Anomalus.Items
{
    public static class InventoryExtensions
    {
        public static bool CanRemoveInventoryItem(this IEnumerable<Inventory> inventories, ItemStack item)
        {
            return inventories.Any(i => i.HasItem(item));
        }

        public static bool CanAppendInventoryItem(this IEnumerable<Inventory> inventories, ItemStack item)
        {
            return inventories.Any(i => i.CanAddItem(item, null) == item.Count);
        }

        public static void RemoveInventoryItem(this IEnumerable<Inventory> inventories, ItemStack item)
        {
            foreach (var inventory in inventories)
            {
                if (inventory.HasItem(item))
                {
                    inventory.RemoveItem(item);
                    break;
                }
            }
        }

        public static void AppendInventoryItem(this IEnumerable<Inventory> inventories, ItemStack item)
        {
            foreach (var inventory in inventories)
            {
                if (inventory.CanAddItem(item, null) == item.Count)
                {
                    inventory.AppendItem(item);
                    break;
                }
            }
        }
    }
}
