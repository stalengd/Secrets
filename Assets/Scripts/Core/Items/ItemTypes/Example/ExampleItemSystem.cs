using Anomalus.Items.Owner;
using Anomalus.Items.UI;
using Anomalus.UIKit.Tooltip;
using UnityEngine;

namespace Anomalus.Items.ItemTypes.Example
{
    public sealed class ExampleItemSystem : ItemSystem<ExampleItemConfig>
    {
        public override bool IsUsable => true;

        public override bool UseItem(ItemStack stack, ItemsUser itemsUser)
        {
            Debug.Log($"Item {stack} used");
            return true;
        }

        public override Tooltip.ContentBuilder ModifyTooltipContent(ItemStack stack, Tooltip.ContentBuilder builder)
        {
            return builder.AppendItemProperty("EXAMPLE POWER!1!!", Config(stack).Foo);
        }

    }
}
