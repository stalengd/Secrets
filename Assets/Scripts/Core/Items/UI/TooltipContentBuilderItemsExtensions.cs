using Anomalus.UIKit.Tooltip;

namespace Anomalus.Items.UI
{
    public static class TooltipContentBuilderItemsExtensions
    {
        public static Tooltip.ContentBuilder AppendItemProperty
            (this Tooltip.ContentBuilder builder, string propertyName, object value, Tooltip.ContentBuilder.TextSprite? icon = null)
        {
            return builder.AppendItemProperty(propertyName, value.ToString(), icon);
        }

        public static Tooltip.ContentBuilder AppendItemProperty
            (this Tooltip.ContentBuilder builder, string propertyName, string value, Tooltip.ContentBuilder.TextSprite? icon = null)
        {
            var block = builder.StartItemProperty(propertyName, icon);
            builder.Append(value);
            block.Dispose();
            return builder;
        }

        public static ItemPropertyBlock StartItemProperty
            (this Tooltip.ContentBuilder builder, string propertyName, Tooltip.ContentBuilder.TextSprite? icon = null)
        {
            return new ItemPropertyBlock(builder, propertyName, icon);
        }
    }

    public struct ItemPropertyBlock : System.IDisposable
    {
        private Tooltip.ContentBuilder _builder;

        public ItemPropertyBlock(Tooltip.ContentBuilder builder, string propertyName, Tooltip.ContentBuilder.TextSprite? icon)
        {
            _builder = builder;
            if (icon.HasValue)
            {
                builder.AppendSprite(icon.Value);
            }
            builder.Append(" ");
            builder.Append(propertyName);
            builder.Append(":");
            builder.RightShift();
        }

        public void Dispose()
        {
            _builder.RightShift(false).Dispose();
            _builder.AppendLine();
        }
    }
}
