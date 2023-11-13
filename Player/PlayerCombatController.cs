using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{

    public class PlayerCombatController : MonoBehaviour
    {
        public readonly int hashLightAttack1 = Animator.StringToHash("Light Attack 1");
        public readonly int hashLightAttack2 = Animator.StringToHash("Light Attack 2");
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
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public StaminaStatManager staminaStatManager;
        public StarterAssetsInputs starterAssetsInputs;
        public ClimbController climbController;
        public DodgeController dodgeController;
        public ThirdPersonController thirdPersonController;
        public PlayerParryManager playerParryManager;
        public UIDocumentDialogueWindow uIDocumentDialogueWindow;
        public PlayerShootingManager playerShootingManager;
        public PlayerComponentManager playerComponentManager;


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
            equipmentGraphicsHandler.DeactivateAllHitboxes();
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

            if (thirdPersonController.Grounded)
            {
                if (lightAttackComboIndex > 2)
                {
                    lightAttackComboIndex = 0;
                }

                if (lightAttackComboIndex == 0)
                {
                    playerManager.PlayBusyAnimation(hashLightAttack1);
                }
                else if (lightAttackComboIndex == 1)
                {
                    playerManager.PlayBusyAnimation(hashLightAttack2);
                }
            }
            else
            {
                HandleJumpAttack();
            }

            lightAttackComboIndex++;
            staminaStatManager.DecreaseLightAttackStamina();

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
            animator.Play(hashJumpAttack);
            playerComponentManager.DisableCollisionWithEnemies();
        }

        public void HandleHeavyAttack()
        {
            if (isCombatting || thirdPersonController.Grounded == false)
            {
                return;
            }

            isHeavyAttacking = true;

            playerManager.PlayBusyAnimation(hashHeavyAttack1);

            staminaStatManager.DecreaseHeavyAttackStamina();
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

            return staminaStatManager.HasEnoughStaminaForLightAttack();
        }

        public bool CanHeavyAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            return staminaStatManager.HasEnoughStaminaForHeavyAttack();
        }

        bool CanAttack()
        {
            if (playerManager.IsBusy)
            {
                return false;
            }

            if (menuManager.IsMenuOpen())
            {
                return false;
            }

            if (playerShootingManager.IsShooting())
            {
                return false;
            }

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return false;
            }

            if (dodgeController.IsDodging())
            {
                return false;
            }

            // If in dialogue
            if (uIDocumentDialogueWindow.isActiveAndEnabled)
            {
                return false;
            }

            if (playerParryManager.IsBlocking())
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
