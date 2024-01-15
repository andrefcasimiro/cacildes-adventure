using System.Collections;
using AF.Companions;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class AmbushState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;
        public UnityEvent onAmbushBegin;
        public UnityEvent onAmbushFinish;

        [Header("Transitions")]
        public State idleState;

        bool ambushHasBegun = false;
        public bool shouldAwake = false;

        Coroutine ambushCoroutine;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            characterManager.agent.ResetPath();
            characterManager.agent.speed = 0f;
        }

        public override void OnStateExit(StateManager stateManager)
        {
            if (ambushCoroutine != null)
            {
                StopCoroutine(ambushCoroutine);
            }

            onStateExit?.Invoke();
        }
        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            if (shouldAwake)
            {
                return idleState;
            }

            return this;
        }

        public void BeginAmbush(float animationDuration)
        {
            if (ambushHasBegun)
            {
                return;
            }

            ambushHasBegun = true;

            if (ambushCoroutine != null)
            {
                StopCoroutine(ambushCoroutine);
            }

            ambushCoroutine = StartCoroutine(Ambush_Coroutine(animationDuration));
        }

        IEnumerator Ambush_Coroutine(float duration)
        {
            onAmbushBegin?.Invoke();
            yield return new WaitForSeconds(duration);
            onAmbushFinish?.Invoke();
            shouldAwake = true;
        }
    }
}
