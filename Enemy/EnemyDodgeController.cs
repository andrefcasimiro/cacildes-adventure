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
        [Range(0, 100)]
        [Tooltip("0 means never, 100 means always")]
        public int dodgeWeight = 100;
        public float maxDodgeCooldown = 10f;
        [HideInInspector] public float dodgeCooldown = Mathf.Infinity; // Used by the EnemyState_Waiting to control the dodge frequency
        public bool dodgeLeftOrRight = false;
        public string[] customDodgeClips;

        CharacterManager characterManager => GetComponent<CharacterManager>();
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
            if (dodgeLeftOrRight) // && characterManager.IsNavMeshAgentActive())
            {
                var playerIsOnTheLeft = false; //characterManager.PlayerIsOnTheLeft();

                if (playerIsOnTheLeft)
                {
                    LookAtPlayer();
                    // characterManager.facePlayer = false;
                    //characterManager.animator.Play(characterManager.hashDodgeRight);

                    ActivateDodge();
                }
                else
                {
                    LookAtPlayer();
                    // Face playerManager first
                    // characterManager.facePlayer = false;
                    // characterManager.animator.Play(characterManager.hashDodgeLeft);

                    ActivateDodge();
                }

                return;
            }

            if (customDodgeClips.Length > 0)
            {
                //                characterManager.enemyHealthController.DisableHealthHitboxes();

                var dice = Random.Range(0, customDodgeClips.Length);

                characterManager.animator.Play(customDodgeClips[dice]);
            }
        }

        public void ActivateDodge()
        {/*
            if (characterManager.enemy.dodgeSfx != null && characterManager.combatAudioSource != null)
            {
                BGMManager.instance.PlaySoundWithPitchVariation(characterManager.enemy.dodgeSfx, characterManager.combatAudioSource);
            }

            characterManager.enemyHealthController.DisableHealthHitboxes();*/
        }

        public void DeactivateDodge()
        {
            /*
            characterManager.facePlayer = true;
            characterManager.enemyHealthController.EnableHealthHitboxes();*/
        }

        public bool IsDodging()
        {
            return characterManager.animator.GetBool(hashIsDodging);
        }
    }
}
