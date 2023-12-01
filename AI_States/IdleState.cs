using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace AF
{
    public class IdleState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;
        public float agentSpeed = 0;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            characterManager.agent.ResetPath();

            characterManager.agent.speed = agentSpeed;
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }
        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            return this;
        }

    }
}
