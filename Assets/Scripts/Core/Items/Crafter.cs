using System.Collections.Generic;
using UnityEngine;

namespace Anomalus.Items
{
    public class Crafter
    {
        [System.Serializable]
        public struct Recipe
        {
            [field: SerializeField]
            public ItemStack Product { get; private set; }
            [field: SerializeField]
            public List<ItemStack> Resources { get; private set; }

            public Recipe(ItemStack product, List<ItemStack> resources)
            {
                Product = product;
                Resources = resources;
            }
        }

        public IEnumerable<Recipe> Recipes { get; set; }

        public Crafter(IEnumerable<Recipe> recipes)
        {
            Recipes = recipes;
        }

        public bool CanCraft(Recipe recipe, IEnumerable<Inventory> source)
        {
            foreach (var resource in recipe.Resources)
            {
                if (!source.CanRemoveInventoryItem(resource))
                    return false;
            }

            if (!source.CanAppendInventoryItem(recipe.Product))
                return false;

            return true;
        }

        public void Craft(Recipe recipe, IEnumerable<Inventory> source)
        {
            foreach (var resource in recipe.Resources)
            {
                source.RemoveInventoryItem(resource);
            }

            source.AppendInventoryItem(recipe.Product);
        }
    }
}