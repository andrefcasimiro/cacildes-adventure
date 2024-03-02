using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class CompanionWaitingStatusDependant : MonoBehaviour
    {
        public CharacterManager characterManager;

        public bool requireWaitingStatus = false;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        private void Start()
        {
            Evaluate();
            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, Evaluate);
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (
                requireWaitingStatus && companionsDatabase.IsCompanionWaiting(characterManager.GetCharacterID())
                || requireWaitingStatus == false && companionsDatabase.IsCompanionWaiting(characterManager.GetCharacterID()) == false
            )
            {
                isActive = true;
            }

            Utils.UpdateTransformChildren(
                transform,
                isActive
            );
        }
    }
}
