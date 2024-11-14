using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Anomalus.UIKit
{
    public sealed class Fade : MonoBehaviour
    {
        [SerializeField] private FadeAction _actionOnStart = FadeAction.None;
        [SerializeField] private float _currentState;
        [SerializeField] private float _fadeTime = 1f;
        [Space]
        [SerializeField] private Graphic _target;
        [SerializeField] private CanvasGroup _canvasGroupTarget;

        private Coroutine _currentTransition;

        private float Opacity
        {
            get
            {
                if (_target != null)
                {
                    return _target.color.a;
                }
                else if (_canvasGroupTarget != null)
                {
                    return _canvasGroupTarget.alpha;
                }

                return 0f;
            }
            set
            {
                if (_target != null)
                {
                    var c = _target.color;
                    c.a = value;
                    _target.color = c;
                }
                else if (_canvasGroupTarget != null)
                {
                    _canvasGroupTarget.alpha = value;
                }
            }
        }

        public enum FadeAction
        {
            None,
            FadeIn,
            FadeOut
        }

        private void Start()
        {
            Do(_actionOnStart);
        }

        public void Do(FadeAction action)
        {
            switch (action)
            {
                case FadeAction.FadeIn:
                    DoFadeIn();
                    break;
                case FadeAction.FadeOut:
                    DoFadeOut();
                    break;

                default: break;
            }
        }

        public Coroutine DoFadeIn()
        {
            return StartTransition(FadeCoroutine(0f, 1f));
        }

        public Coroutine DoFadeOut()
        {
            return StartTransition(FadeCoroutine(1f, 0f));
        }

        public void JumpTo(float state)
        {
            _currentState = state;
            Opacity = state;
        }

        private Coroutine StartTransition(IEnumerator routine)
        {
            if (_currentTransition != null)
                StopCoroutine(_currentTransition);
            _currentTransition = StartCoroutine(routine);
            return _currentTransition;
        }

        private IEnumerator FadeCoroutine(float from, float to)
        {
            var timer = 1f - Mathf.Abs(_currentState - to);

            while (timer < _fadeTime)
            {
                _currentState = Mathf.Lerp(from, to, timer / _fadeTime);
                Opacity = _currentState;

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            _currentState = to;
            Opacity = to;
        }
    }
}
