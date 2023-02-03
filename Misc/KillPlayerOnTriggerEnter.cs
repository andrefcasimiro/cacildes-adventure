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
                var enemyManager = other.GetComponent<EnemyManager>();

                if (enemyManager != null)
                {
                    BGMManager.instance.PlaySound(waterSfx, enemyManager.combatAudioSource);
                    enemyManager.enemyHealthController.TakeEnvironmentalDamage(9999999);
                }

            }
        }
    }
}
