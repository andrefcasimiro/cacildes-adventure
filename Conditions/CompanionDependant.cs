using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class CompanionDependant : MonoBehaviour
    {
        public CompanionID companionID;

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
                (requireInParty && companionsDatabase.IsInParty(companionID.GetCompanionID()))
                || (requireInParty == false && companionsDatabase.IsInParty(companionID.GetCompanionID()) == false)
            );
        }
    }
}
