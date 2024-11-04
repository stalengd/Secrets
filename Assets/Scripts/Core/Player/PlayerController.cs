using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Anomalus.Player
{
	public sealed class PlayerController : MonoBehaviour
	{
		[SerializeField] private float _moveSpeed = 1f;
		
		//[Inject] private readonly FooService _fooService;
		
		private bool _isMoving;
		private InputAction _moveAction;
		
		public event Action<Vector2> PlayerMoved;
		public event Action PlayerStopped;
		
		private void Start()
		{
			// Get movement actions from InputSystem
			_moveAction = InputSystem.actions.FindAction("Move");
		}

		private void Update()
		{
			// Get movement vector and apply it to the player
			// Also invoke event if we actually moved or we have stopped
			var moveVector = _moveAction.ReadValue<Vector2>().normalized * (Time.deltaTime * _moveSpeed);
			if (moveVector.magnitude > 0f)
			{
				transform.Translate(moveVector);
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
