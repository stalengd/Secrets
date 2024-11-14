using Anomalus.Infrastructure.Configurators;
using Anomalus.Items.UI;
using Anomalus.UIKit.Tooltip;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Main
{
    public sealed class MainUIConfigurator : MonoConfigurator
    {
        [SerializeField] private InventoryScreen _inventoryScreen;
        [SerializeField] private Tooltip _tooltip;

        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_inventoryScreen);
            builder.RegisterComponent(_tooltip);
        }
    }
}
