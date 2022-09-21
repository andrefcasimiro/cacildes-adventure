using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_AllowSaveSystem : EventBase
    {

        public bool canUse = true;

        public override IEnumerator Dispatch()
        {
            yield return null;
        }
    }

}
