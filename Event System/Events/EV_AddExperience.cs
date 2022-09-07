using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_AddExperience : EventBase
    {

        public int experienceToAdd = 1;

        public override IEnumerator Dispatch()
        {
            yield return null;

            PlayerStatsManager.instance.AddExperience(experienceToAdd);
        }
    }

}
