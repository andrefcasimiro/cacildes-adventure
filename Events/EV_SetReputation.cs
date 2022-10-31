using System.Collections;
using UnityEngine;

namespace AF
{
    public enum SetReputation
    {
        INCREASE,
        DECREASE,
        SET
    }

    public class EV_SetReputation : EventBase
    {
        public SetReputation setReputation;

        public int amount = 1;

        public bool showNotificationText = true;

        public AudioClip sfx;

        public override IEnumerator Dispatch()
        {
            if (setReputation == SetReputation.SET) {
                Player.instance.currentReputation = amount;
            }
            else if (setReputation == SetReputation.DECREASE)
            {
                Player.instance.currentReputation -= amount;

                if (showNotificationText)
                {
                }
            }
            else
            {
                Player.instance.currentReputation += amount;
                if (showNotificationText)
                {
                }
            }


            if (sfx != null)
            {
                BGMManager.instance.PlaySound(sfx, null);
            }

            yield return null;
        }
    }

}
