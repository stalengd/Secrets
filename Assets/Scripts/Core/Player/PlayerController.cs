using System;
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;

namespace Secrets.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        //[Inject] private readonly FooService _fooService;
        
        private InputAction _moveAction; 
        
        private void Start()
        {
            // Get movement actions from InputSystem
            _moveAction = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            // Get movement vector and apply it to the player
            var moveVector = _moveAction.ReadValue<Vector2>().normalized * (Time.deltaTime * _moveSpeed);
            transform.Translate(moveVector);
        }
    }
    
}
