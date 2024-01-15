using System.Collections;
using AF.Companions;
using UnityEngine;

namespace AF
{

    public class EV_JoinParty : EventBase
    {
        public CompanionID companionId;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.AddToParty(companionId.companionId);
            yield return null;
        }
    }
}
