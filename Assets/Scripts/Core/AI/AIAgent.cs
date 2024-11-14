using System;
using UnityEngine;

namespace Anomalus.AI
{
	public sealed class AIAgent : MonoBehaviour
	{
		[SerializeField] private float _movementSpeed = 1f;
		private Vector2 _currentDestination;
		private bool _isMoving;
		
		public const float STOPPING_MAGNITUDE_APPROX_SQR = 0.0001f; // 0.01 sqr
		
		public event Action AgentStartedMoving;
		public event Action AgentStopped;
		
		private void Update()
		{
			if (!_isMoving)
				return;

			Vector2 position = transform.position;
			var heading = _currentDestination - position;
			if (heading.sqrMagnitude <= STOPPING_MAGNITUDE_APPROX_SQR)
			{
				StopMovement();
				return;
			}
			
			// Do the actual movement here
			var moveVector = heading.normalized * Time.deltaTime * _movementSpeed;
			transform.Translate(moveVector);
		}
		
		private void StartMovement(Vector2 destination)
		{
			_currentDestination = destination;
			_isMoving = true;
			AgentStartedMoving?.Invoke();
		}
		
		public void StopMovement()
		{
			// Discard the destination and stop movement
			_currentDestination = Vector2.zero;
			_isMoving = false;
			AgentStopped?.Invoke();
		}
		
		public void MoveTo(Vector2 destination, bool force = false)
		{
			if (_isMoving && !force)
				return;
			
			StartMovement(destination);
		}
		
		public void MoveTo(Transform destination, bool force = false)
		{
			if (_isMoving && !force)
				return;
			
			StartMovement(destination.position);
		}
		
	}
}