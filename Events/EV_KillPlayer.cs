using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_KillPlayer : EventBase
    {
        public override IEnumerator Dispatch()
        {

            FindObjectOfType<PlayerHealthbox>(true).Die();
            yield return null;
        }
    }

}
