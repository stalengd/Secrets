using System.Collections.Generic;
using UnityEngine;

namespace Anomalus.Items.Ground
{
    public sealed class GroundItemsRegistry
    {
        private readonly HashSet<GroundItem> _available = new();

        public void Add(GroundItem item)
        {
            _available.Add(item);
        }

        public void Remove(GroundItem item)
        {
            _available.Remove(item);
        }

        public void Collect(ICollection<GroundItem> collector, Vector2 center, float range)
        {
            foreach (var item in _available)
            {
                if (CanBeCollected(item, center, range))
                {
                    collector.Add(item);
                }
            }
            //foreach (var item in collector)
            //{
            //    available.Remove(item);
            //}
        }

        private bool CanBeCollected(GroundItem item, Vector2 center, float range)
        {
            var direction = (Vector2)item.transform.position - center;
            return direction.sqrMagnitude <= range * range;
        }
    }
}