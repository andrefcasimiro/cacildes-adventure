using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class CombatState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;


        [Header("States")]
        public State chaseState;
        public State patrolOrIdleState;

        [Header("Events")]
        public UnityEvent onAttack;


        [Header("Events")]
        public UnityEvent onStateEnter;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            characterManager.agent.speed = 0f;

            if (characterManager.IsBusy())
            {
                return this;
            }

            // If Has Target
            if (characterManager.targetManager.currentTarget != null)
            {
                Utils.FaceTarget(characterManager.transform, characterManager.targetManager.currentTarget.transform);

                // If Target Is Still Alive, Attack If Within Range Or Chase
                if (characterManager.targetManager.currentTarget.health.GetCurrentHealth() > 0)
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
                // If Target Is Dead, Forget Target And Return to Idle or Patrol
                else
                {
                    characterManager.targetManager.ClearTarget();
                }
            }

            return patrolOrIdleState;
        }
    }
}
