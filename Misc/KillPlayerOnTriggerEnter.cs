using System.Collections.Generic;
using System.Linq;
using AF.Music;
using UnityEngine;

namespace AF
{
    public class KillPlayerOnTriggerEnter : MonoBehaviour
    {
        public AudioClip waterSfx;

        [Header("Respawn Options")]
        public bool respawnInstead = false;

        public Transform respawnPoint;

        [Header("Ignore Options")]
        public bool ignoreEnemies = false;
        public CharacterManager[] enemiesToIgnore;

        [Header("Components")]
        public BGMManager bgmManager;
        public SceneSettings sceneSettings;

        [Header("Settings")]
        [HideInInspector] public int damageTaken = 9999999;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerManager playerManager = other.GetComponent<PlayerManager>();
                bgmManager.PlaySound(waterSfx, other.GetComponent<PlayerManager>().combatAudioSource);

                if (respawnInstead)
                {
                    playerManager.thirdPersonController.trackFallDamage = false;
                    playerManager.thirdPersonController.isSliding = false;
                    playerManager.thirdPersonController.isSlidingOnIce = false;
                    playerManager.playerComponentManager.UpdatePosition(respawnPoint.transform.position, Quaternion.identity);
                    Instantiate(sceneSettings.respawnFx, respawnPoint.transform.position, Quaternion.identity);
                    playerManager.thirdPersonController.trackFallDamage = true;
                    Physics.autoSyncTransforms = false;
                    return;
                }

                playerManager.health.TakeDamage(damageTaken);
            }
            else if (other.gameObject.CompareTag("Enemy") && ignoreEnemies == false)
            {
                if (other.TryGetComponent<CharacterManager>(out var characterManager))
                {
                    if (enemiesToIgnore != null && enemiesToIgnore.Length > 0)
                    {
                        if (enemiesToIgnore.Contains(characterManager))
                        {
                            return;
                        }
                    }
                    bgmManager.PlaySound(waterSfx, characterManager.combatAudioSource);
                    characterManager.health.TakeDamage(damageTaken);
                }
            }
        }
    }
}
