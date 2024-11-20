using System;
using System.Collections.Generic;
using Anomalus.Pathfinding;
using UnityEngine;
using VContainer;

namespace Anomalus.AI
{
    public sealed class AIAgent : MonoBehaviour
    {
        private const float STOPPING_MAGNITUDE_APPROX_SQR = 0.0025f; // 0.05 sqr
        private const float PATHFINDER_REFRESH_TIME = 0.05f;

        [SerializeField] private float _movementSpeed = 1f;
        private float _pathfinderRefreshTimer;
        private List<Vector2> _currentPath = new();
        private int _currentPointIndex;
        private Vector2 _currentDestination;
        private bool _isMoving;
        private bool _isPathfinding;
        private bool _isPathRefreshEnabled;

        [Inject] private readonly Pathfinder _pathfinder;

        public event Action AgentStartedMoving;
        public event Action AgentStopped;

        private void FixedUpdate()
        {
            // abominatogus
            if (!_isPathfinding &&
                _isMoving &&
                _isPathRefreshEnabled &&
                _currentDestination.sqrMagnitude > STOPPING_MAGNITUDE_APPROX_SQR)
            {
                _pathfinderRefreshTimer += Time.deltaTime;
                if (_pathfinderRefreshTimer >= PATHFINDER_REFRESH_TIME)
                {
                    FindPath(_currentDestination);
                    _pathfinderRefreshTimer = 0f;
                }
            }

            if (!_isMoving)
                return;

            MoveToPoint();
        }

        private void OnDrawGizmosSelected()
        {
            if (!_isMoving)
                return;

            Gizmos.color = Color.blue;
            for (int i = 0; i < _currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(_currentPath[i], _currentPath[i + 1]);
            }
        }

        private void MoveToPoint()
        {
            var heading = GetCurrentHeading();
            if (heading.sqrMagnitude <= STOPPING_MAGNITUDE_APPROX_SQR)
            {
                OnPointReached();
                return;
            }

            // Do the actual movement here
            var moveVector = heading.normalized * Time.deltaTime * _movementSpeed;
            transform.Translate(moveVector);
        }

        private Vector2 GetCurrentHeading()
        {
            return _currentPath[_currentPointIndex] - new Vector2(transform.position.x, transform.position.y);
        }

        private void OnPathFound(Pathfinder.Path path)
        {
            _currentPath.Clear();
            _currentPointIndex = 0;
            _isPathfinding = false;
            _isPathRefreshEnabled = true;

            foreach (var point in path.Points)
            {
                var pos = _pathfinder.Map.ToWorldPosition(point);
                _currentPath.Add(pos);
            }

            if (!_isMoving)
                StartMovement();
        }

        private void OnPointReached()
        {
            _currentPointIndex++;

            if (_currentPath.Count == _currentPointIndex)
            {
                StopMovement();
                return;
            }
        }

        private void FindPath(Vector2 destination)
        {
            _isPathfinding = true;
            _pathfinder.FindPath(transform.position, destination, OnPathFound);
        }

        /// <summary>
        /// Starts (continues) movement if there's any path. To move properly, use MoveTo() method.
        /// </summary>
        public void StartMovement()
        {
            if (_isMoving || _currentPath.Count == 0)
                return;

            _isMoving = true;
            _isPathRefreshEnabled = true;
            AgentStartedMoving?.Invoke();
        }

        public void StopMovement()
        {
            _isMoving = false;
            _isPathRefreshEnabled = false;
            AgentStopped?.Invoke();
        }

        public void MoveTo(Vector2 destination, bool force = false)
        {
            if (_isMoving && !force)
                return;

            _currentDestination = destination;
            FindPath(destination);
        }

        public void MoveTo(Transform destination, bool force = false)
        {
            if (_isMoving && !force)
                return;

            _currentDestination = destination.position;
            FindPath(destination.position);
        }

    }
}