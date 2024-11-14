using System.Collections.Generic;
using UnityEngine;

namespace Anomalus.AI
{
	public sealed class WaypointMovement : MonoBehaviour
	{
		[SerializeField] private AIAgent _aiAgent;
		/// <summary>
		/// Enumerator, which stores the sequence of waypoints. Current holds the waypoint, which we are moving to or the one we are waiting at.
		/// </summary>
		private IEnumerator<Waypoint> _waypoints;
		private Vector2 _position => transform.position;
		private float _currentTimer;
		private bool _stopped;
		
		private void Start()
		{
			_aiAgent.AgentStopped += OnAgentStopped;
		}
		
		private void OnDestroy()
		{
			_aiAgent.AgentStopped -= OnAgentStopped;
		}
		
		private void Update() 
		{
			if (!_stopped)
				return;
				
			_currentTimer += Time.deltaTime;
			if (_currentTimer < _waypoints.Current.StopTime)
				return;
			
			// It's time to go to the next one...
			MoveToNextWaypoint();
		}
		
		private void OnAgentStopped()
		{
			var heading = _waypoints.Current.Position - _position;
			if (heading.sqrMagnitude > AIAgent.STOPPING_MAGNITUDE_APPROX_SQR)
				return;
			
			_stopped = true;
		}
		
		private void MoveToNextWaypoint()
		{
			// If we've reached the end - reset and then move to the first element
			if (!_waypoints.MoveNext())
			{
				_waypoints.Reset();
				_waypoints.MoveNext();
			}

			MoveToCurrentWaypoint();
			ResetVariables();
		}
		
		private void StartMovingOnPath()
		{
			// Reset variables just in case
			ResetVariables();
			
			var nearestWaypoint = GetNearestWaypoint();
			while (_waypoints.MoveNext())
			{
				if (_waypoints.Current.Position != nearestWaypoint.Position)
					continue;
				
				// This way we keep the position
				MoveToCurrentWaypoint();
				break;
			}
		}
		
		private Waypoint GetNearestWaypoint()
		{
			Waypoint closestWaypoint = null;
			var closestWaypointMagnitudeSqr = 0f;
			
			while (_waypoints.MoveNext())
			{
				var heading = _waypoints.Current.Position - _position;
				var sqrMagnitude = heading.sqrMagnitude;
				if (sqrMagnitude > closestWaypointMagnitudeSqr)
				{
					closestWaypointMagnitudeSqr = sqrMagnitude;
					closestWaypoint = _waypoints.Current;
				}
			}
			_waypoints.Reset();
			
			return closestWaypoint ?? _waypoints.Current;
		}
		
		private void ResetVariables()
		{
			_currentTimer = 0f;
			_stopped = false;
		}
		
		private void MoveToWaypoint(Waypoint waypoint) => _aiAgent.MoveTo(waypoint.Position);
		
		public void MoveToCurrentWaypoint() => MoveToWaypoint(_waypoints.Current);
		
		public void StopMovement()
		{
			_aiAgent.StopMovement();
			// Reset those so we ain't breaking the system when we stop our movement at a waiting stop
			ResetVariables();
		}
		
		public bool TrySetPath(List<Waypoint> waypoints, bool startMoving)
		{
			if (waypoints.Count <= 1)
				return false;
			
			_waypoints = waypoints.GetEnumerator();
			if (startMoving)
				StartMovingOnPath();
			
			return true;
		}
	}
}