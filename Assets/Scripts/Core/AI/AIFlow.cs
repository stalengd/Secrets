using Anomalus.Appearance;
using UnityEngine;

namespace Anomalus.AI
{
    public sealed class AIFlow : MonoBehaviour
    {
        [SerializeField] private AIAgent _aiAgent;
        [SerializeField] private AIVision _aiVision;
        [SerializeField] private WaypointMovement _waypointMovement;
        [SerializeField] private CharacterAnimationController _characterAnimationController;

        private void Start()
        {
            _aiAgent.AgentStopped += _waypointMovement.OnAgentStopped;
            _aiAgent.AgentStopped += _characterAnimationController.OnStoppedMovement;
            _aiAgent.AgentMoved += _characterAnimationController.OnMovement;
            _characterAnimationController.SpriteFlipped += _aiVision.OnSpriteFlip;
        }

        private void OnDestroy()
        {
            _aiAgent.AgentStopped -= _waypointMovement.OnAgentStopped;
            _aiAgent.AgentStopped -= _characterAnimationController.OnStoppedMovement;
            _aiAgent.AgentMoved -= _characterAnimationController.OnMovement;
            _characterAnimationController.SpriteFlipped -= _aiVision.OnSpriteFlip;
        }

    }
}