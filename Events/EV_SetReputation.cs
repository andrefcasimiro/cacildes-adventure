using System.Collections;
using System.Collections.Generic;
using AF.Reputation;
using UnityEngine;

namespace AF
{

    public class EV_SetReputation : EventBase
    {
        public bool increase = false;
        public bool decrease = false;
        public int value = 0;

        public override IEnumerator Dispatch()
        {
            if (increase)
            {
                FindAnyObjectByType<PlayerReputation>(FindObjectsInactive.Include).IncreaseReputation(value);
            }
            else if (decrease)
            {
                FindAnyObjectByType<PlayerReputation>(FindObjectsInactive.Include).DecreaseReputation(value);
            }

            yield return null;
        }
    }

}
