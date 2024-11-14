using UnityEngine;

namespace Anomalus.Items.UI
{
    public class InventoryUICellSelectable : InventoryUICell
    {
        [Space]
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                BodyImage.sprite = _isSelected ? _selectedSprite : _defaultSprite;
            }
        }
        private bool _isSelected;
    }
}
