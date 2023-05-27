using StarterAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class KillPlayerOnTriggerEnter : MonoBehaviour
    {
        public AudioClip waterSfx;

        public bool respawnInstead = false;

        SceneSettings sceneSettings;

        PlayerComponentManager playerComponentManager;

        public Transform respawnPoint;

        List<EnemyBossController> enemyBossControllers = new();

        private void Awake()
        {
            enemyBossControllers = FindObjectsOfType<EnemyBossController>(true).ToList();

            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
            sceneSettings = FindObjectOfType<SceneSettings>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                BGMManager.instance.PlaySound(waterSfx, other.GetComponent<PlayerCombatController>().combatAudioSource);

                var noBossInCombat = enemyBossControllers.Count <= 0 || enemyBossControllers.All(x => x.fogWall.activeSelf == false);
                if (respawnInstead && noBossInCombat)
                {
                    playerComponentManager.GetComponent<ThirdPersonController>().trackFallDamage = false;
                    playerComponentManager.GetComponent<ThirdPersonController>().isSliding = false;
                    playerComponentManager.UpdatePosition(respawnPoint.transform.position, Quaternion.identity);
                    Instantiate(sceneSettings.respawnFx, respawnPoint.transform.position, Quaternion.identity);
                    playerComponentManager.GetComponent<ThirdPersonController>().trackFallDamage = true;
                    return;
                }   

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
