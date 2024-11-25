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
        [SerializeField] private ContactFilter2D _targetMask;
        [SerializeField] private LayerMask _obstructionMask;

        /// <summary>
        /// Raised when this AI agent sees someone (something).
        /// </summary>
        public event Action<List<Collider2D>> OnAIVision;

        private void Start()
        {

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
                var direction = (collider.transform.position - transform.position).normalized;
                if (Vector2.Angle(transform.position, direction) < _viewAngle / 2f)
                {
                    var distance = Vector2.Distance(transform.position, collider.transform.position);
                    var raycastHit = Physics2D.Raycast(transform.position, direction, distance, _obstructionMask);
                    if (raycastHit)
                        collidersInSight.Add(collider);
                }
            }

            Debug.Log(collidersInSight.Count());
            if (collidersInSight.Count() > 0)
                OnAIVision?.Invoke(collidersInSight);
        }
    }
}