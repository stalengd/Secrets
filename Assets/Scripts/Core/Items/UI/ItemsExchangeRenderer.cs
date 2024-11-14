using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Items.UI
{
    public class ItemsExchangeRenderer : MonoBehaviour
    {
        [SerializeField] private ItemRenderer _productRenderer;
        [SerializeField] private ItemRenderer _costRenderer;

        [Inject] private readonly IObjectResolver _objectResolver;

        private System.Action _onClicked;

        public void Render(IEnumerable<ItemStack> products, IEnumerable<ItemStack> resources, System.Action onClicked)
        {
            _onClicked = onClicked;

            RenderItems(_productRenderer, products);
            RenderItems(_costRenderer, resources);
        }

        public void Pressed()
        {
            _onClicked?.Invoke();
        }

        private void RenderItems(ItemRenderer sourceRenderer, IEnumerable<ItemStack> items)
        {
            if (!items.Any())
            {
                sourceRenderer.Render(null);
                return;
            }

            var first = items.First();

            foreach (var item in items.Skip(1))
            {
                var renderer = _objectResolver.Instantiate(sourceRenderer, sourceRenderer.transform.parent);
                renderer.Render(item);
            }

            sourceRenderer.Render(first);
        }
    }
}
