using UnityEngine;

namespace Anomalus.Items
{
    [CreateAssetMenu(menuName = "Game Data/Items/Item Type Specialization")]
    public sealed class ItemTypeSpecialization : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Color _color;
        public Color Color => _color;
    }
}
