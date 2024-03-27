using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{

    public class FleeState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Flee Settings")]
        public float maxFleeDistance = 20f;
        public float maxIntervalBetweenDecidingFleeActions = 5f;
        public float fleeSpeed = 5f;

        [Header("States")]
        public State patrolOrIdleState;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onFlee;

        float currentIntervalBetweenFleeActions = 0f;
        Vector3 fleeDestination;

        public override void OnStateEnter(StateManager stateManager)
        {
            currentIntervalBetweenFleeActions = 0f;
            ChooseFleeDestination();

            onStateEnter?.Invoke();
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            characterManager.agent.speed = fleeSpeed;

            if (characterManager.IsBusy())
            {
                return this;
            }

            characterManager.agent.SetDestination(fleeDestination);

            float distanceToFleeDestination = Vector3.Distance(characterManager.transform.position, fleeDestination);

            if (distanceToFleeDestination <= characterManager.agent.stoppingDistance)
            {
                ChooseFleeDestination();
                return this;
            }

            currentIntervalBetweenFleeActions += Time.deltaTime;
            if (currentIntervalBetweenFleeActions >= maxIntervalBetweenDecidingFleeActions)
            {
                currentIntervalBetweenFleeActions = 0f;
                ChooseFleeDestination();
            }

            return this;
        }

        void ChooseFleeDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxFleeDistance;
            randomDirection += characterManager.transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, maxFleeDistance, -1);
            fleeDestination = navHit.position;
            onFlee?.Invoke();
        }
    }
}
