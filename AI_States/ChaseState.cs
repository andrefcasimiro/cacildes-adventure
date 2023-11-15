using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class ChaseState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Chase Settings")]
        public float maxChaseDistance = 20f;

        [Header("States")]
        public State patrolOrIdleState;
        public CombatState combatState;


        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onTargetReached;
        public UnityEvent onTargetLost;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (characterManager.targetManager.CurrentTarget != null)
            {
                // Calculate the distance between the agent and the target
                float distanceToTarget = Vector3.Distance(characterManager.agent.transform.position, characterManager.targetManager.CurrentTarget.transform.position);

                if (distanceToTarget < characterManager.agent.stoppingDistance)
                {
                    // We have reached the target
                    onTargetReached.Invoke();
                    return combatState;
                }
                else if (distanceToTarget > maxChaseDistance)
                {
                    onTargetLost?.Invoke();
                    return patrolOrIdleState;
                }

                if (characterManager.agent.enabled)
                {
                    characterManager.agent.SetDestination(characterManager.targetManager.CurrentTarget.transform.position);
                }
            }

            return this;
        }
    }
}
