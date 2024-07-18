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
        BGMManager bgmManager;
        SceneSettings sceneSettings;

        [Header("Settings")]
        [HideInInspector] public int damageTaken = 9999999;

        BGMManager GetBGMManager()
        {
            if (bgmManager == null)
            {
                bgmManager = FindAnyObjectByType<BGMManager>(FindObjectsInactive.Include);
            }

            return bgmManager;
        }
        SceneSettings GetSceneSettings()
        {
            if (sceneSettings == null)
            {
                sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);
            }

            return sceneSettings;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerManager playerManager = other.GetComponent<PlayerManager>();
                GetBGMManager().PlaySound(waterSfx, other.GetComponent<PlayerManager>().combatAudioSource);

                if (respawnInstead)
                {
                    playerManager.thirdPersonController.SetTrackFallDamage(false);
                    playerManager.thirdPersonController.isSliding = false;
                    playerManager.playerComponentManager.UpdatePosition(respawnPoint.transform.position, Quaternion.identity);
                    Instantiate(GetSceneSettings().respawnFx, respawnPoint.transform.position, Quaternion.identity);
                    playerManager.thirdPersonController.SetTrackFallDamage(true);
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
                    GetBGMManager().PlaySound(waterSfx, characterManager.combatAudioSource);
                    characterManager.health.TakeDamage(damageTaken);
                }
            }
        }
    }
}
