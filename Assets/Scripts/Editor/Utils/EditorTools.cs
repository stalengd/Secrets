using UnityEngine;
using UnityEditor;

namespace Anomalus.Utils
{
    internal struct PropertyQuery
    {
        private SerializedProperty root;

        public PropertyQuery(SerializedProperty root)
        {
            this.root = root;
        }

        public SerializedProperty this[string path]
        {
            get => root.FindPropertyRelative(path);
        }
    }

    internal struct GUIRect
    {
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
        public Vector2 Size
        {
            get => size;
            set => size = value;
        }

        public float X
        {
            get => position.x;
            set => position.x = value;
        }
        public float Y
        {
            get => position.y;
            set => position.y = value;
        }

        public float Width
        {
            get => size.x;
            set => size.x = value;
        }
        public float Height
        {
            get => size.y;
            set => size.y = value;
        }

        private Vector2 position;
        private Vector2 size;


        public GUIRect(Rect rect)
        {
            position = rect.position;
            size = rect.size;
        }


        public Rect ToRect() => new(Position, Size);

        public bool Field(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.PropertyField(ToRect(), property, label);
        }


        public void NextHorizontal()
        {
            Position += Vector2.right * Size.x;
        }

        public void NextVertical()
        {
            Position += Vector2.up * Size.y;
        }

        public void ResetXToFull(Rect fullRect)
        {
            X = fullRect.x;
            Width = fullRect.width;
        }


        public static implicit operator Rect(GUIRect guiRect)
        {
            return guiRect.ToRect();
        }
    }
}
