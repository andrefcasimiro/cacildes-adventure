using System.Collections;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class EV_MarkCompanionToWait : EventBase
    {
        public Companion companion;


        public override IEnumerator Dispatch()
        {
            yield return null;

            var allCompanions = FindObjectsOfType<CompanionManager>(true);
            var c = allCompanions.FirstOrDefault(x => x.companion == companion);

            if (c.waitingForPlayer)
            {
                //                FindObjectOfType<CompanionsSceneManager>(true).UnmarkCompanionAsWaiting(companion);
            }
            else
            {
                //              FindObjectOfType<CompanionsSceneManager>(true).MarkCompanionAsWaiting(companion);
            }

        }

    }
}