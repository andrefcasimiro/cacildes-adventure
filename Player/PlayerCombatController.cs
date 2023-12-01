using System.Collections;
using AF.Ladders;
using AF.Shooting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{

    public class PlayerCombatController : MonoBehaviour
    {
        public readonly int hashLightAttack1 = Animator.StringToHash("Light Attack 1");
        public readonly int hashLightAttack2 = Animator.StringToHash("Light Attack 2");
        public readonly int hashLightAttack1WithShield = Animator.StringToHash("Light Attack 1 With Shield");
        public readonly int hashLightAttack2WithShield = Animator.StringToHash("Light Attack 2 With Shield");
        public readonly int hashHeavyAttack1 = Animator.StringToHash("Heavy Attack 1");
        public readonly int hashJumpAttack = Animator.StringToHash("Jump Attack");
        public readonly int hashIsJumpAttacking = Animator.StringToHash("IsJumpAttacking");
        public readonly int hashIsStartingJumpAttack = Animator.StringToHash("IsStartingJumpAttack");

        [Header("Sounds")]
        public AudioSource combatAudioSource;

        public AudioClip unarmedSwingSfx;
        public AudioClip unarmedHitImpactSfx;

        [Header("Attack Combo Index")]
        public float maxIdleCombo = 2f;
        [SerializeField] int lightAttackComboIndex = 0;
        [SerializeField] int heavyAttackComboIndex = 0;

        [Header("Flags")]
        public bool isCombatting = false;

        [Header("Components")]
        public PlayerManager playerManager;
        public Animator animator;
        public UIDocumentDialogueWindow uIDocumentDialogueWindow;


        [Header("Heavy Attacks")]
        public int unarmedHeavyAttackBonus = 35;

        [Header("UI")]
        public MenuManager menuManager;

        [Header("Flags")]
        public bool isHeavyAttacking = false;
        public bool isDamagingHimself = false;

        // Coroutines
        Coroutine ResetLightAttackComboIndexCoroutine, ResetHeavyAttackComboIndexCoroutine;

        private void Start()
        {
            playerManager.equipmentGraphicsHandler.DeactivateAllHitboxes();
        }

        public void OnLightAttack()
        {
            if (CanLightAttack())
            {
                HandleLightAttack();
            }
        }

        public void OnHeavyAttack()
        {
            if (CanHeavyAttack())
            {
                HandleHeavyAttack();
            }
        }

        public void HandleLightAttack()
        {
            isHeavyAttacking = false;

            if (playerManager.thirdPersonController.Grounded)
            {
                if (lightAttackComboIndex > 2)
                {
                    lightAttackComboIndex = 0;
                }


                if (lightAttackComboIndex == 0)
                {
                    if (playerManager.playerWeaponsManager.currentShieldInstance != null)
                    {
                        playerManager.PlayBusyHashedAnimationWithRootMotion(hashLightAttack1WithShield);
                    }
                    else
                    {
                        playerManager.PlayBusyHashedAnimationWithRootMotion(hashLightAttack1);
                    }
                }
                else if (lightAttackComboIndex == 1)
                {
                    if (playerManager.playerWeaponsManager.currentShieldInstance != null)
                    {
                        playerManager.PlayBusyHashedAnimationWithRootMotion(hashLightAttack2WithShield);

                    }
                    else
                    {
                        playerManager.PlayBusyHashedAnimationWithRootMotion(hashLightAttack2);
                    }
                }
            }
            else
            {
                HandleJumpAttack();
            }

            lightAttackComboIndex++;
            playerManager.staminaStatManager.DecreaseLightAttackStamina();

            if (ResetLightAttackComboIndexCoroutine != null)
            {
                StopCoroutine(ResetLightAttackComboIndexCoroutine);
            }
            ResetLightAttackComboIndexCoroutine = StartCoroutine(_ResetLightAttackComboIndex());
        }

        IEnumerator _ResetLightAttackComboIndex()
        {
            yield return new WaitForSeconds(maxIdleCombo);
            lightAttackComboIndex = 0;
        }

        void HandleJumpAttack()
        {
            playerManager.playerWeaponsManager.HideShield();

            playerManager.playerAnimationEventListener.OpenRightWeaponHitbox();

            playerManager.PlayBusyHashedAnimationWithRootMotion(hashJumpAttack);
            playerManager.playerComponentManager.DisableCollisionWithEnemies();
        }

        public void HandleHeavyAttack()
        {
            if (isCombatting || playerManager.thirdPersonController.Grounded == false)
            {
                return;
            }

            isHeavyAttacking = true;

            playerManager.playerWeaponsManager.HideShield();

            playerManager.PlayBusyHashedAnimationWithRootMotion(hashHeavyAttack1);

            playerManager.staminaStatManager.DecreaseHeavyAttackStamina();
            heavyAttackComboIndex++;

            if (ResetHeavyAttackComboIndexCoroutine != null)
            {
                StopCoroutine(ResetHeavyAttackComboIndexCoroutine);
            }
            ResetHeavyAttackComboIndexCoroutine = StartCoroutine(_ResetHeavyAttackComboIndex());
        }

        IEnumerator _ResetHeavyAttackComboIndex()
        {
            yield return new WaitForSeconds(maxIdleCombo);
            heavyAttackComboIndex = 0;
        }

        public bool CanLightAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForLightAttack();
        }

        public bool CanHeavyAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForHeavyAttack();
        }

        bool CanAttack()
        {
            if (playerManager.IsBusy())
            {
                return false;
            }

            if (playerManager.characterBlockController.isBlocking)
            {
                return false;
            }

            if (menuManager.IsMenuOpen())
            {
                return false;
            }

            if (playerManager.playerShootingManager.isAiming)
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            if (playerManager.dodgeController.IsDodging())
            {
                return false;
            }

            // If in dialogue
            if (uIDocumentDialogueWindow.isActiveAndEnabled)
            {
                return false;
            }


            return true;
        }

        public bool IsJumpAttacking()
        {
            return false;
            //            return animator.GetBool(hashIsJumpAttacking);
        }

        public bool IsStartingJumpAttack()
        {
            return false;
            //            return animator.GetBool(hashIsStartingJumpAttack);
        }

        private void OnDisable()
        {
            animator.SetBool(hashIsJumpAttacking, false);
            animator.SetBool(hashIsStartingJumpAttack, false);
            isHeavyAttacking = false;
        }
    }
}
