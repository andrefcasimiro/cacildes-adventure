using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class KillPlayerOnTriggerEnter : MonoBehaviour
    {
        public AudioClip waterSfx;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                BGMManager.instance.PlaySound(waterSfx, other.GetComponent<PlayerCombatController>().combatAudioSource);
                other.GetComponentInChildren<PlayerHealthbox>(true).Die();
            }

            if (other.gameObject.tag == "Enemy")
            {
                BGMManager.instance.PlaySound(waterSfx, other.GetComponent<EnemyCombatController>().combatAudioSource);
                other.GetComponent<EnemyHealthController>().TakeEnvironmentalDamage(Mathf.Infinity);
            }
        }
    }

}
