using Anomalus.Utils;
using UnityEngine;
using TMPro;

namespace Anomalus.UIKit
{
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private ManualTimer _timeToLive = new(1f);
        [SerializeField] private AnimationCurve _opacityCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public void Init(float damageAmount)
        {
            _text.text = damageAmount.ToString("N0");
        }

        private void Update()
        {
            if (_timeToLive.Update())
            {
                Destroy(gameObject);
            }

            _text.color = _text.color.WithAlpha(_opacityCurve.Evaluate(_timeToLive.Progress));
            transform.position += _speedCurve.Evaluate(_timeToLive.Progress) * Time.deltaTime * Vector3.up;
        }
    }
}
