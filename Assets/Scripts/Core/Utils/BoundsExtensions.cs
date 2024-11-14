using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomalus.Utils
{
    public static class BoundsExtensions
    {
        public static Bounds ToBounds(this IEnumerable<Vector3> positions)
        {
            if (positions == null)
                throw new System.ArgumentNullException(nameof(positions));

            var first = positions.FirstOrDefault();
            var bounds = new Bounds(first, Vector3.zero);

            foreach (var position in positions)
            {
                bounds.Encapsulate(position);
            }

            return bounds;
        }

        public static Bounds ToBounds(this IEnumerable<Vector2> positions)
        {
            if (positions == null)
                throw new System.ArgumentNullException(nameof(positions));

            var first = positions.FirstOrDefault();
            var bounds = new Bounds(first, Vector3.zero);

            foreach (var position in positions)
            {
                bounds.Encapsulate(position);
            }

            return bounds;
        }

        public static Bounds ToBounds(this IEnumerable<Rect> rects)
        {
            if (rects == null)
                throw new System.ArgumentNullException(nameof(rects));

            var first = rects.FirstOrDefault();
            var bounds = new Bounds(first.center, first.size);

            foreach (var rect in rects)
            {
                bounds.Encapsulate(rect.min);
                bounds.Encapsulate(rect.max);
            }

            return bounds;
        }

        public static Vector3 RandomPoint(this Bounds bounds)
        {
            return bounds.min.BoxRandom(bounds.max);
        }
    }
}
