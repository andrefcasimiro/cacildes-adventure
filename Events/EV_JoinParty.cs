using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

namespace AF
{

    public class EV_JoinParty : EventBase
    {
        public Companion companionToJoin;

        public override IEnumerator Dispatch()
        {
            yield return null;

            FindObjectOfType<CompanionsSceneManager>(true).AddCompanionToParty(companionToJoin);
        }

    }

}
