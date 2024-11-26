using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomalus.AI
{
    public sealed class AIVision : MonoBehaviour
    {
        private const int MAX_OVERLAP_RESULTS = 50;
        [SerializeField, Range(0f, 360f)] private float _viewAngle = 15f;
        [SerializeField] private float _viewRadius = 10f;
        private Vector2 _facingDirection;
        [SerializeField] private ContactFilter2D _targetMask;
        [SerializeField] private LayerMask _obstructionMask;

        /// <summary>
        /// Raised when this AI agent sees someone (something).
        /// </summary>
        public event Action<List<Collider2D>> OnAIVision;

        private void Start()
        {
            _facingDirection = transform.right; // we imagine that we look right by default
        }

        private void FixedUpdate()
        {
            Collider2D[] possibleColliders = new Collider2D[MAX_OVERLAP_RESULTS];
            var collidersCount = Physics2D.OverlapCircle(transform.position, _viewRadius, _targetMask, possibleColliders);
            var colliders = new Collider2D[collidersCount];
            for (int i = 0; i < collidersCount; i++)
                colliders[i] = possibleColliders[i];

            if (colliders.Count() == 0)
                return;

            var collidersInSight = new List<Collider2D>();
            foreach (var collider in colliders)
            {
                Vector2 targetDirection = (collider.transform.position - transform.position).normalized;
                var angle = Vector2.Angle(_facingDirection, targetDirection);
                if (angle < _viewAngle / 2f)
                {
                    var distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (!Physics2D.Raycast(transform.position, targetDirection, distance, _obstructionMask))
                        collidersInSight.Add(collider);
                }
            }

            Debug.Log(collidersInSight.Count());
            if (collidersInSight.Count() > 0)
                OnAIVision?.Invoke(collidersInSight);
        }

        public void OnMovement(Vector2 movingVector)
        {
            _facingDirection = movingVector.normalized;
        }
    }
}