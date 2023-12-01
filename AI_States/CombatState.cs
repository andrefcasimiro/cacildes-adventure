using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class CombatState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Agent Settings")]
        public float agentSpeed = 0f;

        [Header("States")]
        public ChaseState chaseState;

        [Header("Events")]
        public UnityEvent onAttack;


        [Header("Events")]
        public UnityEvent onStateEnter;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            characterManager.agent.speed = agentSpeed;
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
                float distanceToTarget = Vector3.Distance(
                    characterManager.agent.transform.position, characterManager.targetManager.currentTarget.transform.position);

                if (distanceToTarget <= characterManager.agent.stoppingDistance)
                {
                    onAttack?.Invoke();

                    return this;
                }
                else if (distanceToTarget > characterManager.agent.stoppingDistance)
                {
                    return chaseState;
                }
            }

            return this;
        }
    }
}
