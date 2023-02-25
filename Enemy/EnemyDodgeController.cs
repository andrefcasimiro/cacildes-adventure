using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EnemyDodgeController : MonoBehaviour
    {

        [Header("Dodge Settings")]
        [Range(0, 100)][Tooltip("0 means never, 100 means always")]
        public int dodgeWeight = 100;
        public float maxDodgeCooldown = 10f;
        [HideInInspector] public float dodgeCooldown = Mathf.Infinity; // Used by the EnemyState_Waiting to control the dodge frequency
        public bool dodgeLeftOrRight = false;
        public string[] customDodgeClips;
        public float dodgeDistanceBasedOnAnimations = 3f;

        EnemyManager enemyManager => GetComponent<EnemyManager>();
        PlayerCombatController playerCombatController => FindObjectOfType<PlayerCombatController>(true);

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

                    if (chance <= dodgeWeight)
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
                    NavMeshPath navMeshPath = new NavMeshPath();
                    var Target = transform.position + transform.right * dodgeDistanceBasedOnAnimations;
                    var canReach = enemyManager.agent.CalculatePath(Target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete;
                    if (canReach)
                    {
                        // When dodging, disable agent because it will interfer with root motion logic and produce weird dodge movementsF
                        enemyManager.agent.enabled = false;
                        LookAtPlayer();
                        enemyManager.facePlayer = false;
                        enemyManager.enemyHealthController.DisableHealthHitboxes();
                        enemyManager.animator.CrossFade(enemyManager.hashDodgeRight, 0.1f);
                        return;
                    }
                }
                else
                {
                    NavMeshPath navMeshPath = new NavMeshPath();
                    var Target = transform.position + transform.right * -dodgeDistanceBasedOnAnimations;
                    var canReach = enemyManager.agent.CalculatePath(Target, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete;

                    if (canReach)
                    {
                        // When dodging, disable agent because it will interfer with root motion logic and produce weird dodge movementsF
                        enemyManager.agent.enabled = false;

                        LookAtPlayer();
                        // Face playerManager first
                        enemyManager.facePlayer = false;
                        enemyManager.enemyHealthController.DisableHealthHitboxes();
                        enemyManager.animator.CrossFade(enemyManager.hashDodgeLeft, 0.1f);
                        return;
                    }
                }

                return;
            }

            if (customDodgeClips.Length > 0)
            {
                enemyManager.enemyHealthController.DisableHealthHitboxes();

                var dice = Random.Range(0, customDodgeClips.Length);

                enemyManager.animator.CrossFade(customDodgeClips[dice], 0.2f);
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
    }
}
