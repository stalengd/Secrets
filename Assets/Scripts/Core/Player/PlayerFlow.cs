using Secrets.Appearance;
using UnityEngine;

namespace Secrets.Player
{
	public class PlayerFlow : MonoBehaviour
	{
		[SerializeField] private PlayerController _playerController;
		[SerializeField] private CharacterAnimationController _characterAnimationController;
		
		private void Start()
		{
			_playerController.PlayerMoved += _characterAnimationController.OnMovement;
			_playerController.PlayerStopped += _characterAnimationController.OnStoppedMovement;
		}
		
		private void OnDestroy()
		{
			_playerController.PlayerMoved -= _characterAnimationController.OnMovement;
			_playerController.PlayerStopped -= _characterAnimationController.OnStoppedMovement;
		}
	}
}
