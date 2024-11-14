using Anomalus.UIKit.Tooltip;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;

namespace Anomalus.Items.UI
{
    public sealed class ItemRenderer : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _cornerImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private TooltipOnHover _tooltip;

        [Inject] private readonly ItemSystemManager _itemSystemManager;

        public Color IconColor
        {
            get => _iconImage.color;
            set => _iconImage.color = value;
        }

        public Color CountColor
        {
            get => _countText.color;
            set => _countText.color = value;
        }

        public ItemStack Stack { get; private set; }
        public bool IsEmpty { get; private set; }

        //private void Awake()
        //{
        //    LocalizationSettings.SelectedLocaleChanged += SelectedLocaleChanged;
        //}

        //private void OnDestroy()
        //{
        //    LocalizationSettings.SelectedLocaleChanged -= SelectedLocaleChanged;
        //}

        //private void SelectedLocaleChanged(Locale locale)
        //{
        //    if (!IsEmpty)
        //    {
        //        if (nameText != null) nameText.text = Stack.ItemType.Name;

        //        tooltip.Content = GenerateTooltipContent(Stack.ItemType);
        //    }
        //}

        public void Render(ItemStack? stack)
        {
            if (stack.HasValue)
            {
                Stack = stack.Value;
                IsEmpty = false;

                _countText.text = Stack.Count > 1 ? Stack.Count.ToString() : "";
                if (_nameText != null) _nameText.text = Stack.ItemType.Name;
                _iconImage.sprite = Stack.ItemType.Sprite;
                _iconImage.enabled = true;

                if (_tooltip != null)
                    _tooltip.Content = GenerateTooltipContent(Stack);

                if (Stack.ItemType.Specialization != null)
                {
                    var color = Stack.ItemType.Specialization.Color;

                    if (_bgImage != null)
                    {
                        _bgImage.enabled = true;
                        _bgImage.color = new Color(color.r, color.g, color.b) { a = _bgImage.color.a };
                    }
                    if (_cornerImage != null)
                    {
                        _cornerImage.enabled = true;
                        _cornerImage.color = new Color(color.r, color.g, color.b) { a = _cornerImage.color.a };
                    }
                }
            }
            else
            {
                _countText.text = "";
                if (_nameText != null) _nameText.text = "";
                _iconImage.enabled = false;
                if (_tooltip != null)
                {
                    _tooltip.Content = null;
                }
                if (_bgImage != null)
                {
                    _bgImage.enabled = false;
                }
                if (_cornerImage != null)
                {
                    _cornerImage.enabled = false;
                }
                IsEmpty = true;
            }
        }

        private Tooltip.Content? GenerateTooltipContent(ItemStack stack)
        {
            var data = stack.ItemType;
            if (data == null)
                return null;

            var builder = new Tooltip.ContentBuilder()
                .AppendTitle(data.Name);
            if (data.Specialization is { } specialization)
            {
                builder = builder.AppendColor(data.Specialization.Color, data.Specialization.Name);
            }
            builder = builder.AppendLine();
            builder = _itemSystemManager.Get(stack).ModifyTooltipContent(stack, builder);

            using (builder.Opacity(200))
            {
                builder = builder
                    .AppendVerticalSpace(0.5f)
                    .AppendLine(data.Description);
            }
            return builder;
        }
    }
}
