using UnityEngine;

namespace Anomalus.AI
{
    public sealed class AIFlow : MonoBehaviour
    {
        [SerializeField] private AIAgent _aiAgent;
        [SerializeField] private WaypointMovement _waypointMovement;

        private void Start()
        {
            _aiAgent.AgentStopped += _waypointMovement.OnAgentStopped;
        }

        private void OnDestroy()
        {
            _aiAgent.AgentStopped -= _waypointMovement.OnAgentStopped;
        }

    }
}