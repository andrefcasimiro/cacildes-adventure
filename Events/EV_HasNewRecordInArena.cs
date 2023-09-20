using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EV_HasNewRecordInArena : EV_Condition
    {

        public override IEnumerator Dispatch()
        {
            yield return DispatchConditionResults(FindAnyObjectByType<RoundManager>(FindObjectsInactive.Include).HasBeatenNewRecord());
        }
    }

}
