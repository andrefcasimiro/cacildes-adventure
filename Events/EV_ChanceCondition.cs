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

        bool finalValue;

        public override IEnumerator Dispatch()
        {
            if (hasRun && runOnlyOnce)
            {
                yield return DispatchConditionResults(finalValue);
            }
            else
            {
                finalValue = Random.Range(0, 100) <= chance;

                if (runOnlyOnce)
                {
                    hasRun = true;

                }

                yield return DispatchConditionResults(finalValue);
            }
        }
    }

}