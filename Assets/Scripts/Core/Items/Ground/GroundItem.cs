using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Anomalus.Items.Ground
{
    public sealed class GroundItem : MonoBehaviour
    {
        [SerializeField] private ItemStack _item;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private float _activationTime = 1f;
        [SerializeField] private UnityEvent _onMagnetized;

        [Inject] private readonly GroundItemsRegistry _registry;

        public ItemStack Item
        {
            get => _item;
            set
            {
                _item = value;
                if (_item.ItemType != null && _renderer != null)
                {
                    _renderer.sprite = _item.ItemType.Sprite;
                }
            }
        }
        public UnityEvent OnMagnetized => _onMagnetized;

        private bool _isAddedToRegistry = false;

        private IEnumerator Start()
        {
            Item = _item;

            yield return new WaitForSeconds(_activationTime);

            _registry.Add(this);
            _isAddedToRegistry = true;
        }

        private void OnDestroy()
        {
            RemoveFromRegistry();
        }

        public void StartPull()
        {
            RemoveFromRegistry();
            _onMagnetized.Invoke();
        }

        private void RemoveFromRegistry()
        {
            if (_isAddedToRegistry)
            {
                _registry.Remove(this);
                _isAddedToRegistry = false;
            }
        }
    }
}
