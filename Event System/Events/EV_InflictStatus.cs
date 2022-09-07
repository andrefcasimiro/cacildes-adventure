using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_InflictStatus : EventBase
    {
        public StatusEffect statusEffect;
        public float amount;

        public override IEnumerator Dispatch()
        {
            PlayerStatsManager.instance.UpdateStatusEffect(statusEffect, amount);

            yield return null;
        }
    }

}
