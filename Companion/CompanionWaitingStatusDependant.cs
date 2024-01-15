using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Companions
{
    public class CompanionWaitingStatusDependant : MonoBehaviour
    {
        public CompanionID companionId;

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
                requireWaitingStatus && companionsDatabase.IsCompanionWaiting(companionId.companionId)
                || requireWaitingStatus == false && companionsDatabase.IsCompanionWaiting(companionId.companionId) == false
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
