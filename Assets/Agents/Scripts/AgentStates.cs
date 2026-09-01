using UnityEngine;

namespace Cinderwild.Gameplay.Agents
{
    public enum AgentState
    {
        Idle,
        Move
    }

    public class AgentStates : MonoBehaviour
    {
        private AgentState current = AgentState.Idle;

        public AgentState Current => current;
        public void SetState(AgentState state) { current = state; }
        private Agent agent;
        public void Init(Agent agent)
        {
            this.agent = agent;
        }

        private void Update()
        {
            HandleStates();
        }

        private void HandleStates()
        {
            switch (current)
            {
                case AgentState.Idle:
                    IdleState();
                    break;
                case AgentState.Move:
                    MoveState();
                    break;
            }
        }

        private void IdleState()
        {

        }

        private void MoveState()
        {

        }
    }
}
