using UnityEngine;

namespace Anomalus.Appearance
{
	public sealed class CharacterAnimationController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		public void OnMovement(Vector2 moveVector)
		{
			_animator.SetBool("Running", true);
			
			if (moveVector.x != 0f)
				_spriteRenderer.flipX = moveVector.x < 0f;
		}
		
		public void OnStoppedMovement()
		{
			_animator.SetBool("Running", false);
		}
	}
}