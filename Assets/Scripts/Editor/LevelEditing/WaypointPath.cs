using Anomalus.Scientists;
using UnityEditor;
using UnityEngine;

namespace Anomalus.LevelEditing
{
    [CustomEditor(typeof(ScientistSpawnPreset)), CanEditMultipleObjects]
    public sealed class WaypointPath : Editor
    {
        private void OnSceneGUI()
        {
            var t = target as ScientistSpawnPreset;
            foreach (var waypoint in t.MovementWaypoints)
            {
                EditorGUI.BeginChangeCheck();
                var newPosition = Handles.PositionHandle(waypoint.Position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Change waypoint position");
                    waypoint.Position = newPosition;
                }
            }
        }
    }
}
