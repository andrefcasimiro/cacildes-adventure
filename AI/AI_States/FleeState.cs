using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class FleeState : State
    {
        [Header("Components")]
        [SerializeField] private CharacterManager characterManager;

        [Header("Flee Settings")]
        [SerializeField] private float maxFleeDistance = 20f;
        [SerializeField] private float maxIntervalBetweenDecidingFleeActions = 5f;
        [SerializeField] private float fleeSpeed = 5f;

        [Header("States")]
        [SerializeField] private State patrolOrIdleState;

        [Header("Events")]
        [SerializeField] private UnityEvent onStateEnter;
        [SerializeField] private UnityEvent onFlee;

        private float currentIntervalBetweenFleeActions;
        private Vector3 fleeDestination;

        [Header("Go To Other State After X Time Options")]
        [SerializeField] private bool shouldReturnToGivenStateAfterSomeTime = false;
        [SerializeField] private State stateToReturnAfterTimeHasPassed;
        [SerializeField] private float maxTimeFleeingBeforeExitingToOtherState = 15f;
        private float fleeTimer;

        public override void OnStateEnter(StateManager stateManager)
        {
            ResetFleeTimers();
            ChooseFleeDestination();
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

            characterManager.agent.speed = fleeSpeed;
            characterManager.agent.SetDestination(fleeDestination);

            if (ShouldReturnToOtherState())
            {
                return stateToReturnAfterTimeHasPassed;
            }

            if (HasReachedDestination())
            {
                ChooseFleeDestination();
            }

            UpdateFleeTimers();

            return this;
        }

        private void ResetFleeTimers()
        {
            currentIntervalBetweenFleeActions = 0f;
            fleeTimer = 0f;
        }

        private bool ShouldReturnToOtherState()
        {
            if (shouldReturnToGivenStateAfterSomeTime)
            {
                fleeTimer += Time.deltaTime;

                if (fleeTimer > maxTimeFleeingBeforeExitingToOtherState)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasReachedDestination()
        {
            float distanceToFleeDestination = Vector3.Distance(characterManager.transform.position, fleeDestination);
            return distanceToFleeDestination <= characterManager.agent.stoppingDistance;
        }

        private void UpdateFleeTimers()
        {
            currentIntervalBetweenFleeActions += Time.deltaTime;

            if (currentIntervalBetweenFleeActions >= maxIntervalBetweenDecidingFleeActions)
            {
                currentIntervalBetweenFleeActions = 0f;
                ChooseFleeDestination();
            }
        }

        private void ChooseFleeDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxFleeDistance;
            randomDirection += characterManager.transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, maxFleeDistance, NavMesh.AllAreas))
            {
                fleeDestination = navHit.position;
                onFlee?.Invoke();
            }
        }
    }
}
