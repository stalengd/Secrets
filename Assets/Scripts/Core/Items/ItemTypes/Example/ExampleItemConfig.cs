using UnityEngine;

namespace Anomalus.Items.ItemTypes.Example
{
    [CreateAssetMenu(menuName = "Game Data/Items/Types/Example")]
    public sealed class ExampleItemConfig : ItemConfig
    {
        [Header("Example")]
        [SerializeField] private float _foo = 10f;

        public float Foo => _foo;
    }
}
