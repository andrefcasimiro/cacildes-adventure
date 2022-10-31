using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_AddGold : EventBase
    {
        public int goldAmount = 1;

        public override IEnumerator Dispatch()
        {
            Player.instance.currentGold += goldAmount;
            yield return null;
        }
    }

}
