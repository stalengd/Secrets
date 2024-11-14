using UnityEngine;

namespace Anomalus.UIKit.Bars
{
    public class BarResize : Bar
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private Vector2 _direction = Vector2.right;
        [SerializeField] private RectOffset _padding;

        public override void SetValue(float value, float maxValue)
        {
            base.SetValue(value, maxValue);

            var parent = _target.parent as RectTransform;
            var parentSize = parent.rect.size;

            parentSize.x -= _padding.horizontal;
            parentSize.y -= _padding.vertical;

            var size = _target.sizeDelta;
            size.x = Mathf.Lerp(size.x, parentSize.x * (value / maxValue), _direction.x);
            size.y = Mathf.Lerp(size.y, parentSize.y * (value / maxValue), _direction.y);
            _target.sizeDelta = size;
        }
    }
}
