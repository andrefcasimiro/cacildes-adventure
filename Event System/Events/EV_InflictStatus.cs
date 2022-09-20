using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_InflictStatus : EventBase
    {
        public StatusEffect statusEffect;
        public float amount;

        public bool inflictWhileInEventTriggerCollider = false;
        public EventTriggerCollider eventTriggerCollider;

        public override IEnumerator Dispatch()
        {
            if (eventTriggerCollider != null && inflictWhileInEventTriggerCollider)
            {
                while (eventTriggerCollider.playerInCollider)
                {
                    PlayerStatsManager.instance.UpdateStatusEffect(statusEffect, amount);

                    yield return null;
                }
            }


            PlayerStatsManager.instance.UpdateStatusEffect(statusEffect, amount);

            yield return null;
        }
    }

}
