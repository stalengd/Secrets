using UnityEngine;
using TMPro;

namespace Anomalus.UIKit.Bars
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public virtual void SetValue(float value, float maxValue)
        {
            if (text != null)
            {
                text.text = $"{value:F0}/{maxValue:F0}";
            }
        }
    }
}
