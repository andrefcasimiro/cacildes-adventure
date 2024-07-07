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

            if (characterManager.targetManager.currentTarget != null)
            {
                RotateTowardsTarget(characterManager.targetManager.currentTarget.transform);
            }
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (!characterManager.isCuttingDistanceToTarget)
            {
                characterManager.agent.speed = 0f;
            }

            if (characterManager.IsBusy())
            {
                return this;
            }

            if (HasValidTarget())
            {
                return HandleCombatWithTarget();
            }

            return patrolOrIdleState;
        }

        private bool HasValidTarget()
        {
            var target = characterManager.targetManager.currentTarget;
            if (target == null || target.health.GetCurrentHealth() <= 0)
            {
                characterManager.targetManager.ClearTarget();
                return false;
            }

            return true;
        }

        private State HandleCombatWithTarget()
        {
            var target = characterManager.targetManager.currentTarget.transform;
            float distanceToTarget = Vector3.Distance(characterManager.agent.transform.position, target.position);

            if (distanceToTarget <= characterManager.agent.stoppingDistance)
            {
                onAttack?.Invoke();
                return this;
            }
            else
            {
                return chaseState;
            }
        }

        private void RotateTowardsTarget(Transform target)
        {
            Vector3 directionToLook = target.position - characterManager.transform.position;
            directionToLook.y = 0; // Keep the y value 0 to avoid tilting

            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            characterManager.transform.rotation = Quaternion.Slerp(
                characterManager.transform.rotation,
                targetRotation,
                characterManager.rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}
