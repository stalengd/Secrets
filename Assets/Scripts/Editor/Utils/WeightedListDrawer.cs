using UnityEngine;
using UnityEditor;

namespace Anomalus.Utils
{
    [CustomPropertyDrawer(typeof(WeightedList<>))]
    public sealed class WeightedListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fullRect = position;
            var r = new GUIRect(fullRect) { Height = EditorGUIUtility.singleLineHeight };

            var q = new PropertyQuery(property);
            var list = q["list"];
            var sumProp = q["sum"];
            var sum = 0f;

            for (int i = 0; i < list.arraySize; i++)
            {
                sum += list.GetArrayElementAtIndex(i).FindPropertyRelative("weight").floatValue;
            }
            sumProp.floatValue = sum;

            r.Field(list, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var q = new PropertyQuery(property);
            return EditorGUI.GetPropertyHeight(q["list"], true);
        }
    }

    [CustomPropertyDrawer(typeof(WeightedList<>.Item))]
    public sealed class WeightedListItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var q = new PropertyQuery(property);
            var weight = q["weight"];

            var sum
                = property.serializedObject.FindProperty($"{property.propertyPath[..property.propertyPath.LastIndexOf(".list")]}.sum");
            var sumValue = sum.floatValue;

            var chance = sumValue == 0f ? 1f : weight.floatValue / sumValue;

            var fullRect = position;
            var r = new GUIRect(fullRect) { Height = EditorGUIUtility.singleLineHeight };


            GUI.Label(r, new GUIContent(chance.ToString("P2")));

            r.NextVertical();
            r.Field(weight, new GUIContent("Weight"));

            r.NextVertical();
            r.Field(q["value"], GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var q = new PropertyQuery(property);
            return (EditorGUIUtility.singleLineHeight * 2) + EditorGUI.GetPropertyHeight(q["value"], true);
        }
    }
}
