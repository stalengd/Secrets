using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Anomalus.UIKit
{
    [System.Serializable]
    public sealed class ListHelper<T> where T : Component
    {
        public RectTransform Holder;
        public GameObject Prefab;

        public int ItemsCount => _items.Count;

        private readonly List<T> _items = new();
        private readonly Stack<T> _pool = new();

        private IObjectResolver _objectResolver;

        public void UseResolver(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public T CreateItem()
        {
            if (_pool.TryPop(out var item))
            {
                item.transform.SetSiblingIndex(Holder.childCount - 1);
            }
            else
            {
                var go = Instantiate();
                item = go.GetComponent<T>();
            }
            item.gameObject.SetActive(true);

            _items.Add(item);
            return item;
        }

        public void CreateItems(int count, System.Action<int, T> configureItem)
        {
            for (var i = 0; i < count; i++)
            {
                var item = CreateItem();
                configureItem(count, item);
            }
        }

        public void CreateItems(int count, System.Action<T> configureItem)
        {
            for (var i = 0; i < count; i++)
            {
                var item = CreateItem();
                configureItem(item);
            }
        }

        public void CreateItems<TModel>(IEnumerable<TModel> source, System.Action<TModel, T> configureItem)
        {
            foreach (var model in source)
            {
                var item = CreateItem();
                configureItem(model, item);
            }
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                item.gameObject.SetActive(false);
                _pool.Push(item);
            }
            _items.Clear();
        }

        private GameObject Instantiate()
        {
            return _objectResolver is { }
                ? _objectResolver.Instantiate(Prefab, Holder)
                : Object.Instantiate(Prefab, Holder);
        }
    }
}
