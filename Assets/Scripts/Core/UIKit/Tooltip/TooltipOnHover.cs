using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Anomalus.UIKit.Tooltip
{
    public sealed class TooltipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Tooltip.Content _content;

        public Tooltip.Content? Content
        {
            get => _hasContent ? _content : null;
            set
            {
                _hasContent = value.HasValue;
                if (_hasContent)
                {
                    _content = value.Value;
                }
                if (!_isShowed)
                {
                    return;
                }
                if (_hasContent)
                {
                    _tooltip.Show(Input.mousePosition, _content);
                }
                else
                {
                    _tooltip.Hide();
                    _isShowed = false;
                }
            }
        }

        [Inject] private readonly Tooltip _tooltip;
        private bool _isShowed = false;
        private bool _hasContent = false;

        private void OnDisable()
        {
            if (_isShowed)
            {
                _tooltip.Hide();
                _isShowed = false;
            }
        }

        private void Update()
        {
            if (_isShowed)
                _tooltip.MoveTo(Input.mousePosition);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Content.HasValue && Cursor.visible)
            {
                _tooltip.Show(eventData.position, Content.Value);
                _isShowed = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltip.Hide();
            _isShowed = false;
        }

        public void SetContentText(string text)
        {
            Content = new Tooltip.Content(text);
        }
    }
}
