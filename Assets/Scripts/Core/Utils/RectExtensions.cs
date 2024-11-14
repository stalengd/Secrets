using UnityEngine;

namespace Anomalus.Utils
{
    public static class RectExtensions
    {
        public static Vector2 RandomPoint(this Rect rect)
        {
            return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
        }

        public static Vector2 ClampPoint(this Rect rect, Vector2 point)
        {
            point.x = Mathf.Clamp(point.x, rect.xMin, rect.xMax);
            point.y = Mathf.Clamp(point.y, rect.yMin, rect.yMax);
            return point;
        }

        public static Rect Offset(this Rect rect, Vector2 distance)
        {
            rect.position += distance;
            return rect;
        }

        public static Rect WithWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }
    }
}
