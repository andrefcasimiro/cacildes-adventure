using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EnemyDodgeController : MonoBehaviour
    {
        [HideInInspector] public int hashIsDodging = Animator.StringToHash("IsDodging");

        [Header("Dodge Settings")]
        [Range(0, 100)][Tooltip("0 means never, 100 means always")]
        public int dodgeWeight = 100;
        public float maxDodgeCooldown = 10f;
        [HideInInspector] public float dodgeCooldown = Mathf.Infinity; // Used by the EnemyState_Waiting to control the dodge frequency
        public bool dodgeLeftOrRight = false;
        public string[] customDodgeClips;

        EnemyManager enemyManager => GetComponent<EnemyManager>();
        PlayerCombatController playerCombatController;

        private void Awake()
        {
            playerCombatController = FindObjectOfType<PlayerCombatController>(true);
        }

        void Update()
        {
            UpdateDodgeCounter();
        }
        
        void UpdateDodgeCounter()
        {
            if (dodgeCooldown < maxDodgeCooldown)
            {
                dodgeCooldown += Time.deltaTime;
            }
        }

        public void CheckForDodgeChance()
        {
            // If playerManager is attacking, evaluate if enemy can dodge attack
            if (playerCombatController.isCombatting)
            {
                if (dodgeCooldown >= maxDodgeCooldown)
                {
                    dodgeCooldown = 0f;

                    // Roll dodge dice
                    float chance = Random.Range(0, 100);

                    if (chance <= dodgeWeight && dodgeWeight != 0)
                    {
                        Dodge();
                    }
                }
            }
        }

        public void LookAtPlayer()
        {
            Utils.FaceTarget(transform, playerCombatController.transform);
        }

        public void Dodge()
        {
            if (dodgeLeftOrRight && enemyManager.IsNavMeshAgentActive())
            {
                var playerIsOnTheLeft = enemyManager.PlayerIsOnTheLeft();

                if (playerIsOnTheLeft)
                {
                    LookAtPlayer();
                    enemyManager.facePlayer = false;
                    enemyManager.animator.Play(enemyManager.hashDodgeRight);

                    ActivateDodge();
                }
                else
                {
                    LookAtPlayer();
                    // Face playerManager first
                    enemyManager.facePlayer = false;
                    enemyManager.animator.Play(enemyManager.hashDodgeLeft);

                    ActivateDodge();
                }

                return;
            }

            if (customDodgeClips.Length > 0)
            {
                enemyManager.enemyHealthController.DisableHealthHitboxes();

                var dice = Random.Range(0, customDodgeClips.Length);

                enemyManager.animator.Play(customDodgeClips[dice]);
            }
        }

        public void ActivateDodge()
        {
            if (enemyManager.enemy.dodgeSfx != null && enemyManager.combatAudioSource != null)
            {
                BGMManager.instance.PlaySoundWithPitchVariation(enemyManager.enemy.dodgeSfx, enemyManager.combatAudioSource);
            }

            enemyManager.enemyHealthController.DisableHealthHitboxes();
        }

        public void DeactivateDodge()
        {
            enemyManager.facePlayer = true;
            enemyManager.enemyHealthController.EnableHealthHitboxes();
        }

        public bool IsDodging()
        {
            return enemyManager.animator.GetBool(hashIsDodging);
        }
    }
}
