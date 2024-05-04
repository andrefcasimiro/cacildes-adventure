using AF.Companions;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class IdleState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        [Header("Companion Settings")]
        PlayerManager playerManager;

        public CompanionsDatabase companionsDatabase;
        public State chaseState;

        private void Awake()
        {
            if (characterManager.IsCompanion())
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            characterManager.agent.ResetPath();
            characterManager.agent.speed = 0f;
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }
        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            if (ShouldFollowPlayer())
            {
                return chaseState;
            }

            return this;
        }

        bool ShouldFollowPlayer()
        {
            if (characterManager.IsCompanion() == false)
            {
                return false;
            }

            if (companionsDatabase.IsCompanionAndIsActivelyInParty(characterManager.GetCharacterID()))
            {
                return Vector3.Distance(characterManager.agent.transform.position, playerManager.transform.position)
                    > characterManager.agent.stoppingDistance + companionsDatabase.companionToPlayerStoppingDistance;
            }

            return false;
        }
    }
}
