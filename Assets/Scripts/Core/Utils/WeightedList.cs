using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomalus.Utils
{
    [System.Serializable]
    public class WeightedList<T> : IEnumerable<WeightedList<T>.Item>
    {
        [SerializeField] private List<Item> _list = new();
        [SerializeField] private float _sum; // for editor purposes

        [System.Serializable]
        public struct Item
        {
            [SerializeField] private float _weight;
            [SerializeField] private T _value;
            public float Weight => _weight;
            public T Value => _value;

            public Item(int weight, T value)
            {
                _weight = weight;
                _value = value;
            }
        }

        public int ItemsCount => _list.Count;
        public IReadOnlyList<Item> List => _list;

        public T GetRandomValue()
        {
            var index = GetRandomWeightIndex();

            return _list[index].Value;
        }

        public T GetRandomValue(System.Func<float, float, float> randomGenerator)
        {
            var index = GetRandomWeightIndex(randomGenerator);

            return _list[index].Value;
        }

        public int GetRandomWeightIndex()
        {
            return GetRandomWeightIndex(Random.Range);
        }

        public int GetRandomWeightIndex(System.Func<float, float, float> randomGenerator)
        {
            var sum = 0f;
            foreach (var item in _list)
            {
                sum += item.Weight;
            }
            var rnd = randomGenerator(0, sum);
            for (var i = 0; i < _list.Count; i++)
            {
                if (rnd <= _list[i].Weight) return i;
                rnd -= _list[i].Weight;
            }
            return -1;
        }

        public void Add(int weight, T value)
        {
            _list.Add(new Item(weight, value));
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }

    [System.Serializable]
    public class CustomWeightedList<T> : IEnumerable<T>
    {
        [SerializeField] private List<T> _list = new();

        public int ItemsCount => _list.Count;
        public IReadOnlyList<T> List => _list;

        public CustomWeightedList(List<T> list)
        {
            _list = list;
        }

        public T GetRandomValue(System.Func<T, float> weightSelector)
        {
            var index = GetRandomWeightIndex(weightSelector);

            return _list[index];
        }

        public T GetRandomValue(System.Func<T, float> weightSelector, System.Func<float, float, float> randomGenerator)
        {
            var index = GetRandomWeightIndex(weightSelector, randomGenerator);

            return _list[index];
        }

        public int GetRandomWeightIndex(System.Func<T, float> weightSelector)
        {
            return GetRandomWeightIndex(weightSelector, Random.Range);
        }

        public int GetRandomWeightIndex(System.Func<T, float> weightSelector, System.Func<float, float, float> randomGenerator)
        {
            return WeightedListUtil.GetRandomWeightIndex(_list.Select(weightSelector), randomGenerator);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }

    public static class WeightedListUtil
    {
        public static int GetRandomWeightIndex(IEnumerable<int> weights, System.Func<int, int, int> randomGenerator)
        {
            var sum = weights.Sum();
            var count = weights.Count();
            var rnd = randomGenerator(0, sum + 1);
            for (var i = 0; i < count; i++)
            {
                var w = weights.ElementAt(i);
                if (rnd <= w) return i;
                rnd -= w;
            }
            return -1;
        }

        public static int GetRandomWeightIndex(IEnumerable<float> weights, System.Func<float, float, float> randomGenerator)
        {
            var sum = weights.Sum();
            var count = weights.Count();
            var rnd = randomGenerator(0f, sum);
            for (var i = 0; i < count; i++)
            {
                var w = weights.ElementAt(i);
                if (rnd <= w) return i;
                rnd -= w;
            }
            return -1;
        }
    }
}
