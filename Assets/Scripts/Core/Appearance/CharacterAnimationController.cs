using System;
using UnityEngine;

namespace Anomalus.Appearance
{
    public sealed class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Raised when sprite flips. Args is whether the sprite is currently flipped (faced left).
        /// </summary>
        public Action<bool> SpriteFlipped;

        public void OnMovement(Vector2 moveVector)
        {
            _animator.SetBool("Running", true);

            if (moveVector.x != 0f)
            {
                var flip = moveVector.x < 0f;
                _spriteRenderer.flipX = flip;
                SpriteFlipped?.Invoke(flip);
            }
        }

        public void OnStoppedMovement()
        {
            _animator.SetBool("Running", false);
        }
    }
}