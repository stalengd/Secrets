using UnityEngine;

namespace Anomalus.Utils
{
    public static class TransformExtensions
    {
        public static Transform ModifyPosition(this Transform transform, System.Func<Vector3, Vector3> func)
        {
            var position = transform.position;
            transform.position = func(position);
            return transform;
        }

        public static Transform ModifyLocalPosition(this Transform transform, System.Func<Vector3, Vector3> func)
        {
            var position = transform.localPosition;
            transform.localPosition = func(position);
            return transform;
        }

        public static Transform ModifyLocalScale(this Transform transform, System.Func<Vector3, Vector3> func)
        {
            var scale = transform.localScale;
            transform.localScale = func(scale);
            return transform;
        }

        public static bool IsScreenPointOver(this RectTransform rectTransform, Vector2 sceenPoint)
        {
            var scale = rectTransform.lossyScale.x;
            var rect = rectTransform.rect;
            rect = new Rect(rect.position * scale, rect.size * scale);
            var bounds = new Bounds((Vector3)rect.center + rectTransform.position, (Vector3)rect.size + Vector3.forward * 100f);
            return bounds.Contains(sceenPoint);
        }
    }
}
