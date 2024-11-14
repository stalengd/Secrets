using System.Collections.Generic;
using System.Linq;
using Anomalus.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.Items.UI
{
    public sealed class CraftUI : MonoBehaviour
    {
        [SerializeField] private GameObject _recipePrefab;
        [SerializeField] private Transform _recipesHolder;
        [SerializeField] private RectTransform _rectBounds;

        [Inject] private readonly IObjectResolver _objectResolver;

        private Crafter _crafter;
        private IEnumerable<Inventory> _inventories;

        public void Render(Crafter crafter, IEnumerable<Inventory> sourceInventories)
        {
            // Destroying and spawning again same objects is not very performant,
            // TODO some object pooling

            foreach (Transform t in _recipesHolder)
            {
                Destroy(t.gameObject);
            }

            foreach (var recipe in crafter.Recipes)
            {
                var renderer = _objectResolver.Instantiate(_recipePrefab, _recipesHolder).GetComponent<ItemsExchangeRenderer>();
                renderer.Render(Enumerable.Repeat(recipe.Product, 1), recipe.Resources, () => RecipePressed(recipe));
            }

            _crafter = crafter;
            _inventories = sourceInventories;
        }

        public void RecipePressed(Crafter.Recipe offer)
        {
            if (_crafter.CanCraft(offer, _inventories))
            {
                _crafter.Craft(offer, _inventories);
            }
        }

        public bool IsMouseOver()
        {
            if (!gameObject.activeInHierarchy) return false;
            return _rectBounds.IsScreenPointOver(Input.mousePosition);
        }
    }
}
