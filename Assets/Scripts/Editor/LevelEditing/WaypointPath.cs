using Anomalus.AI;
using UnityEditor;
using UnityEngine;

namespace Anomalus.LevelEditing
{
	[CustomEditor(typeof(SpawnPreset)), CanEditMultipleObjects]
	public sealed class WaypointPath : Editor
	{
		private void OnSceneGUI()
		{
			var t = target as SpawnPreset;
			foreach(var waypoint in t.MovementWaypoints)
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
