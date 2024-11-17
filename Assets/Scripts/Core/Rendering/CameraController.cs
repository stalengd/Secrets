using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anomalus.Rendering
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private bool _limitView;
        [SerializeField] private Rect _boundaries;

        public Camera Camera => _camera;

        private readonly List<List<Transform>> _targetsStack = new();
        private Vector2 _targetPosition;

        private void Start()
        {
            _targetPosition = transform.position;
            _targetPosition = CalculateTargetPosition();
            var pos = (Vector3)_targetPosition;
            pos.z = transform.position.z;
            transform.position = pos;
        }

        private void FixedUpdate()
        {
            var newTargetPosition = CalculateTargetPosition();
            _targetPosition = Vector2.Lerp(_targetPosition, (Vector2)newTargetPosition, Time.fixedDeltaTime * 5f);

            Vector3 pos = _targetPosition;
            var cameraSize = new Vector2(_camera.orthographicSize * (Screen.width / (float)Screen.height), _camera.orthographicSize);

            if (_limitView)
            {
                var finalBoundaries = new Rect(_boundaries.position + cameraSize, _boundaries.size - cameraSize * 2f);
                if (pos.x < finalBoundaries.xMin)
                    pos.x = finalBoundaries.xMin;
                if (pos.x > finalBoundaries.xMax)
                    pos.x = finalBoundaries.xMax;
                if (pos.y < finalBoundaries.yMin)
                    pos.y = finalBoundaries.yMin;
                if (pos.y > finalBoundaries.yMax)
                    pos.y = finalBoundaries.yMax;
            }

            pos.z = transform.position.z;
            transform.position = pos;
        }

        private void OnDrawGizmos()
        {
            if (_limitView)
            {
                Gizmos.DrawWireCube(_boundaries.center, _boundaries.size);
            }
        }

        public Vector3 CalculateTargetPosition()
        {
            var targets = _targetsStack.LastOrDefault();
            if (targets == null) return _targetPosition;

            var bounds = new Bounds(targets.First().position, Vector3.zero);
            for (var i = 1; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }
            return bounds.center;
        }

        public void Track(List<Transform> transforms)
        {
            if (transforms == null || !transforms.Any()) return;

            _targetsStack.Add(transforms);
        }

        public void Track(Transform transform)
        {
            Track(new List<Transform>() { transform });
        }

        public void Untrack(List<Transform> transforms)
        {
            _targetsStack.Remove(transforms);
        }

        public void Untrack(Transform transform)
        {
            for (var i = _targetsStack.Count - 1; i >= 0; i--)
            {
                if (_targetsStack[i].Contains(transform))
                {
                    _targetsStack.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
