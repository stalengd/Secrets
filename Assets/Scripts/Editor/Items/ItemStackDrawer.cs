using UnityEngine;
using UnityEditor;

namespace Anomalus.Items
{
    [CustomPropertyDrawer(typeof(ItemStack))]
    public sealed class ItemStackDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var countRect = new Rect(position.x, position.y, 40, position.height);
            var itemTypeRect = new Rect(position.x + countRect.width, position.y, position.width - countRect.width, position.height);

            EditorGUI.PropertyField(countRect, property.FindPropertyRelative("_count"), GUIContent.none);
            EditorGUI.PropertyField(itemTypeRect, property.FindPropertyRelative("_itemType"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
