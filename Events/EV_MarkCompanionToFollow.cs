using System.Collections;
using AF.Companions;
using UnityEngine;

namespace AF
{
    public class EV_MarkCompanionToFollow : EventBase
    {
        public CompanionID companionId;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            companionsDatabase.FollowPlayer(companionId.companionId);

            yield return null;
        }
    }
}
