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


        [Header("Chase Actions Settings")]
        public float maxIntervalBetweenDecidingChaseActions = 5f;
        float currentIntervalBetweenChaseActions = 0f;


        public override void OnStateEnter(StateManager stateManager)
        {
            currentIntervalBetweenChaseActions = 0f;

            onStateEnter?.Invoke();
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (characterManager.IsBusy())
            {
                return this;
            }

            if (characterManager.targetManager.currentTarget != null)
            {
                if (characterManager.agent.enabled)
                {
                    characterManager.agent.SetDestination(characterManager.targetManager.currentTarget.transform.position);
                }

                // Calculate the distance between the agent and the target
                float distanceToTarget = Vector3.Distance(characterManager.agent.transform.position, characterManager.targetManager.currentTarget.transform.position);

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

                currentIntervalBetweenChaseActions += Time.deltaTime;
                if (currentIntervalBetweenChaseActions >= maxIntervalBetweenDecidingChaseActions)
                {
                    currentIntervalBetweenChaseActions = 0f;

                    if (characterManager.characterCombatController != null)
                    {
                        characterManager.characterCombatController.UseChaseAction();
                        return this;
                    }
                }
            }

            return this;
        }
    }
}
