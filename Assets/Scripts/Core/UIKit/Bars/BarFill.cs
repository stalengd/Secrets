using UnityEngine;
using UnityEngine.UI;

namespace Anomalus.UIKit.Bars
{
    public class BarFill : Bar
    {
        [SerializeField] private Image _fill;

        public override void SetValue(float value, float maxValue)
        {
            base.SetValue(value, maxValue);

            _fill.fillAmount = value / maxValue;
        }
    }
}
