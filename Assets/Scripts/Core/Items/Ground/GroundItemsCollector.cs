using System.Collections.Generic;
using Anomalus.Items.Owner;
using UnityEngine;
using VContainer;

namespace Anomalus.Items.Ground
{
    public class GroundItemsCollector : MonoBehaviour
    {
        [SerializeField] private float _radius = 1f;
        [SerializeField] private float _magnetizedItemsSpeed = 5f;

        [Inject] private readonly GroundItemsRegistry _registry;
        [Inject] private readonly IInventoryOwner _inventoryOwner;

        private readonly List<GroundItem> _bufferList = new();
        private readonly Dictionary<GroundItem, ItemStack> _magnetizedList = new();
        private float _speedMultiplier = 1f;
        private float _acceleration = 1f;

        private void FixedUpdate()
        {
            CollectFromRegistry();
            MoveMagnetizedItems();

            if (_magnetizedList.Count > 0)
            {
                _speedMultiplier += _acceleration * Time.fixedDeltaTime;
            }
            else
            {
                _speedMultiplier = 1f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        private void OnLevelWasLoaded(int level)
        {
            foreach (var (_, item) in _magnetizedList)
            {
                _inventoryOwner.AllOwnedInventories.AppendInventoryItem(item);
            }
            _magnetizedList.Clear();
        }

        private void CollectFromRegistry()
        {
            _bufferList.Clear();
            _registry.Collect(_bufferList, transform.position, _radius);

            for (var i = 0; i < _bufferList.Count; i++)
            {
                var groundItem = _bufferList[i];
                var item = groundItem.Item;

                if (!_inventoryOwner.AllOwnedInventories.CanAppendInventoryItem(item)) continue;

                groundItem.StartPull();
                _magnetizedList.Add(groundItem, item);
            }
        }

        private void MoveMagnetizedItems()
        {
            _bufferList.Clear();
            var destroyedList = _bufferList;
            foreach (var (groundItem, item) in _magnetizedList)
            {
                var direction = groundItem.transform.position - transform.position;
                direction.z = 0f;
                var minDistance = _magnetizedItemsSpeed * _speedMultiplier * Time.fixedDeltaTime * 5f;
                if (direction.sqrMagnitude <= minDistance * minDistance)
                {
                    destroyedList.Add(groundItem);
                    Destroy(groundItem.gameObject);
                    _inventoryOwner.AllOwnedInventories.AppendInventoryItem(item);

                    continue;
                }

                groundItem.transform.position = Vector3.MoveTowards(
                    groundItem.transform.position,
                    transform.position,
                    _magnetizedItemsSpeed * _speedMultiplier * Time.fixedDeltaTime);
            }
            foreach (var item in destroyedList)
            {
                _magnetizedList.Remove(item);
            }
        }
    }
}
