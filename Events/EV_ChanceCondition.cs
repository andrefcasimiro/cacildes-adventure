using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ChanceCondition : EV_Condition
    {
        [Header("Chance")]
        [Range(0, 100)] public float chance = 0f;

        [Header("Options")]
        public bool runOnlyOnce = false;
        public bool hasRun = false;

        public override IEnumerator Dispatch()
        {
            if (hasRun && runOnlyOnce)
            {
                yield return null;
            }
            else
            {
                bool finalValue = Random.Range(0, 100) <= chance;

                if (runOnlyOnce)
                {
                    hasRun = true;
                }

                yield return DispatchConditionResults(finalValue);
            }
        }
    }

}