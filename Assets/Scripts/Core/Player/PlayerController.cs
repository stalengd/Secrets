using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Anomalus.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private Rigidbody2D _rigidbody;

        private bool _isMoving;
        private InputAction _moveAction;

        public event Action<Vector2> PlayerMoved;
        public event Action PlayerStopped;

        private void Start()
        {
            // Get movement actions from InputSystem
            _moveAction = InputSystem.actions.FindAction("Move");
        }

        private void FixedUpdate()
        {
            // Get movement vector and apply it to the player
            // Also invoke event if we actually moved or we have stopped
            var moveVector = _moveAction.ReadValue<Vector2>().normalized * _moveSpeed;
            _rigidbody.linearVelocity = moveVector;
            if (moveVector.magnitude > 0f)
            {
                PlayerMoved?.Invoke(moveVector);
                _isMoving = true;
            }
            else if (moveVector.magnitude <= 0f && _isMoving)
            {
                _isMoving = false;
                PlayerStopped?.Invoke();
            }
        }
    }
}
