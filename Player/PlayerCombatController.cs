using System.Collections;
using System.Numerics;
using AF.Ladders;
using UnityEngine;

namespace AF
{

    public class PlayerCombatController : MonoBehaviour
    {

        public readonly int hashLightAttack1 = Animator.StringToHash("Light Attack 1");
        public readonly int hashLightAttack2 = Animator.StringToHash("Light Attack 2");
        public readonly int hashHeavyAttack1 = Animator.StringToHash("Heavy Attack 1");
        public readonly int hashJumpAttack = Animator.StringToHash("Jump Attack");

        [Header("Sounds")]
        public AudioSource combatAudioSource;

        [Header("Attack Combo Index")]
        public float maxIdleCombo = 2f;
        [SerializeField] int lightAttackComboIndex = 0;
        [SerializeField] int heavyAttackComboIndex = 0;

        [Header("Flags")]
        public bool isCombatting = false;
        public bool isLightAttacking = false;

        [Header("Components")]
        public PlayerManager playerManager;
        public Animator animator;
        public UIDocumentDialogueWindow uIDocumentDialogueWindow;
        public LockOnManager lockOnManager;
        public UIManager uIManager;

        [Header("Heavy Attacks")]
        public int unarmedHeavyAttackBonus = 35;

        [Header("UI")]
        public MenuManager menuManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Flags")]
        public bool isHeavyAttacking = false;
        public bool isJumpAttacking = false;


        // Coroutines
        Coroutine ResetLightAttackComboIndexCoroutine, ResetHeavyAttackComboIndexCoroutine;


        public readonly string SpeedMultiplierHash = "SpeedMultiplier";

        private void Start()
        {
            animator.SetFloat(SpeedMultiplierHash, 1f);
        }

        public void ResetStates()
        {
            isJumpAttacking = false;
            isHeavyAttacking = false;
            isLightAttacking = false;
            animator.SetFloat(SpeedMultiplierHash, 1f);
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

        public bool IsAttacking()
        {
            return isLightAttacking || isHeavyAttacking || isJumpAttacking;
        }

        public void HandleLightAttack()
        {
            isHeavyAttacking = false;
            isLightAttacking = true;

            if (playerManager.thirdPersonController.Grounded)
            {
                if (playerManager.playerBackstabController.PerformBackstab())
                {
                    return;
                }

                if (lightAttackComboIndex > 1)
                {
                    lightAttackComboIndex = 0;
                }

                if (lightAttackComboIndex == 0)
                {
                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashLightAttack1);

                }
                else if (lightAttackComboIndex == 1)
                {
                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashLightAttack2);
                }

                HandleAttackSpeed();
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

        void HandleAttackSpeed()
        {
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();
            if (equipmentDatabase.isTwoHanding == false && currentWeapon != null && currentWeapon.oneHandAttackSpeedPenalty != 1)
            {
                animator.SetFloat(SpeedMultiplierHash, currentWeapon.oneHandAttackSpeedPenalty);
            }
            else
            {
                animator.SetFloat(SpeedMultiplierHash, 1f);
            }
        }

        void HandleJumpAttack()
        {
            isHeavyAttacking = false;
            isLightAttacking = false;
            isJumpAttacking = true;

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

            isLightAttacking = false;
            isHeavyAttacking = true;

            playerManager.playerWeaponsManager.HideShield();

            playerManager.PlayBusyHashedAnimationWithRootMotion(hashHeavyAttack1);

            playerManager.staminaStatManager.DecreaseHeavyAttackStamina();
            heavyAttackComboIndex++;

            //HandleAttackSpeed();

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

            if (equipmentDatabase.IsStaffEquipped() || equipmentDatabase.IsBowEquipped())
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

            if (menuManager.isMenuOpen)
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

            if (playerManager.dodgeController.isDodging)
            {
                return false;
            }

            if (uIManager.IsShowingGUI())
            {
                return false;
            }


            return true;
        }

        private void OnDisable()
        {
            ResetStates();
        }
    }
}
