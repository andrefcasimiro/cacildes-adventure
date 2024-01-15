using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Companions
{
    public class CompanionDependant : MonoBehaviour
    {
        public CompanionID companionId;

        public bool requireInParty = true;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        private void Start()
        {
            Evaluate();

            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, Evaluate);
        }

        public void Evaluate()
        {
            Utils.UpdateTransformChildren(
                transform,
                (requireInParty && companionsDatabase.IsInParty(companionId.companionId))
                || (requireInParty == false && companionsDatabase.IsInParty(companionId.companionId) == false)
            );
        }
    }
}
