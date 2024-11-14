using System;
using Anomalus.Items.Owner;
using Anomalus.UIKit.Tooltip;

namespace Anomalus.Items
{
    public sealed class BasicItemSystem : ItemSystem<ItemConfig>
    {

    }

    public abstract class ItemSystem<T> : ItemSystem where T : ItemConfig
    {
        public override Type ItemType => typeof(T);

        public T Config(ItemStack stack)
        {
            return stack.ItemType as T;
        }
    }

    public abstract class ItemSystem
    {
        public abstract Type ItemType { get; }

        public virtual bool IsUsable => false;

        public virtual bool UseItem(Inventory.Slot slot, ItemsUser itemsUser)
        {
            var r = UseItem(slot.AsStack, itemsUser);
            if (r)
            {
                slot.Count -= 1;
            }
            return r;
        }

        public virtual bool UseItem(ItemStack stack, ItemsUser itemsUser)
        {
            return false;
        }

        public virtual void OnEquipped(ItemStack stack, ItemsUser itemsUser) { }
        public virtual void OnUnequipped(ItemStack stack, ItemsUser itemsUser) { }

        public virtual Tooltip.ContentBuilder ModifyTooltipContent(ItemStack stack, Tooltip.ContentBuilder builder)
        {
            return builder;
        }
    }
}
