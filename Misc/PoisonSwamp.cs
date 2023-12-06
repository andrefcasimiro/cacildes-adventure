using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PoisonSwamp : MonoBehaviour
    {
        [Header("Status Effect")]
        public StatusEffect statusEffect;
        public float amount = 0.1f;


        [Tooltip("If player dodges while in the poisonous swamp, how much damage is added")]
        public float dodgeBonusMultiplier = 2f;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                var finalStatusAmount = amount;
                if (other.GetComponent<DodgeController>().isDodging)
                {
                    finalStatusAmount *= dodgeBonusMultiplier;
                }

            }
        }

    }

}
