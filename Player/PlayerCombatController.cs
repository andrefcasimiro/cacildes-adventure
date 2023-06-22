using UnityEngine;
using StarterAssets;

namespace AF
{

    public class PlayerCombatController : MonoBehaviour
    {
        public readonly int hashCombatting = Animator.StringToHash("IsCombatting");
        public readonly int hashLightAttack1 = Animator.StringToHash("Light Attack A");
        public readonly int hashLightAttack2 = Animator.StringToHash("Light Attack B");
        public readonly int hashLightAttack3 = Animator.StringToHash("Light Attack C");
        public readonly int hashHeavyAttack1 = Animator.StringToHash("Heavy Attack A");
        public readonly int hashHeavyAttack2 = Animator.StringToHash("Heavy Attack B");
        public readonly int hashHeavyAttack3 = Animator.StringToHash("Heavy Attack C");
        public readonly int hashJumpAttack = Animator.StringToHash("Jump Attack");
        public readonly int hashIsJumpAttacking = Animator.StringToHash("IsJumpAttacking");
        public readonly int hashIsStartingJumpAttack = Animator.StringToHash("IsStartingJumpAttack");
        
        [Header("Transform Refs")]
        public Transform headRef;

        [Header("Sounds")]
        public AudioSource combatAudioSource;
        public AudioClip blockingSfx;

        public AudioClip unarmedSwingSfx;
        public AudioClip unarmedHitImpactSfx;

        [Header("Attack Combo Index")]
        public float maxIdleCombo = 2f;
        [SerializeField] int lightAttackComboIndex = 0;
        float timeSinceLastLightAttack = 0f;
        [SerializeField] int heavyAttackComboIndex = 0;
        float timeSinceLastHeavyAttack = 0f;

        [Header("Flags")]
        public bool isCombatting = false;
        
        Animator animator => GetComponent<Animator>();
        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();
        StaminaStatManager staminaStatManager => GetComponent<StaminaStatManager>();
        StarterAssetsInputs starterAssetsInputs => GetComponent<StarterAssetsInputs>();
        ClimbController climbController => GetComponent<ClimbController>();
        DodgeController dodgeController => GetComponent<DodgeController>();
        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();
        PlayerParryManager playerParryManager => GetComponent<PlayerParryManager>();
        UIDocumentDialogueWindow uIDocumentDialogueWindow;
        PlayerShootingManager playerShootingManager => GetComponent<PlayerShootingManager>();

        public bool isParrying = false;

        [Header("Stamina")]
        public int unarmedLightAttackStaminaCost = 15;
        public int unarmedHeavyAttackStaminaCost = 35;

        public WeaponHandlerRef leftWeaponHandlerRef;
        public WeaponHandlerRef rightWeaponHandlerRef;

        public bool isHeavyAttacking = false;

        [Header("Heavy Attacks")]
        public int unarmedHeavyAttackBonus = 35;

        public bool skipIK = false;

        private void Start()
        {
            uIDocumentDialogueWindow = FindObjectOfType<UIDocumentDialogueWindow>(true);
            equipmentGraphicsHandler.DeactivateAllHitboxes();
        }

        public void SkipIK()
        {
            skipIK = true;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (playerParryManager.IsBlocking() && Player.instance.equippedShield != null)
            {
                return;
            }

            if (skipIK)
            {
                return;
            }

            if (leftWeaponHandlerRef != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftWeaponHandlerRef.transform.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftWeaponHandlerRef.transform.rotation);
            }

            if (rightWeaponHandlerRef != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightWeaponHandlerRef.transform.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightWeaponHandlerRef.transform.rotation);
            }
        }

        private void Update()
        {
            isCombatting = animator.GetBool(hashCombatting);

            if (timeSinceLastLightAttack < maxIdleCombo)
            {
                timeSinceLastLightAttack += Time.deltaTime;
            }

            if (timeSinceLastHeavyAttack < maxIdleCombo)
            {
                timeSinceLastHeavyAttack += Time.deltaTime;
            }

            // Light Attack
            if (lightAttackComboIndex != 0 && timeSinceLastLightAttack >= maxIdleCombo)
            {
                lightAttackComboIndex = 0;
            }
            if (heavyAttackComboIndex != 0 && timeSinceLastHeavyAttack >= maxIdleCombo)
            {
                heavyAttackComboIndex = 0;
            }

            if (starterAssetsInputs.lightAttack)
            {
                starterAssetsInputs.lightAttack = false;

                if (CanLightAttack())
                {
                    HandleLightAttack();
                }
            }
            else if (starterAssetsInputs.heavyAttack)
            {
                starterAssetsInputs.heavyAttack = false;
                if (CanHeavyAttack())
                {
                    HandleHeavyAttack();
                }
            }
        }

        public void HandleLightAttack()
        {
            if (isCombatting)
            {
                return;
            }

            isHeavyAttacking = false;


            equipmentGraphicsHandler.ShowWeapons();

            if (thirdPersonController.Grounded)
            {
                if (lightAttackComboIndex > 2)
                {
                    lightAttackComboIndex = 0;
                }

                if (lightAttackComboIndex == 0)
                {
                    animator.CrossFade(hashLightAttack1, 0.05f);
                }
                else if (lightAttackComboIndex == 1)
                {
                    animator.CrossFade(hashLightAttack2, 0.05f);
                }
                else
                {
                    animator.CrossFade(hashLightAttack3, 0.05f);
                }
            }
            else
            {
                animator.Play(hashJumpAttack);
            }

            animator.SetBool(hashCombatting, true);

            staminaStatManager.DecreaseStamina(Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.lightAttackStaminaCost : unarmedLightAttackStaminaCost);

            lightAttackComboIndex++;
            timeSinceLastLightAttack = 0f;
        }

        public void HandleHeavyAttack()
        {
            if (isCombatting || thirdPersonController.Grounded == false)
            {
                return;
            }

            isHeavyAttacking = true;

            equipmentGraphicsHandler.ShowWeapons();

            if (heavyAttackComboIndex > 2)
            {
                heavyAttackComboIndex = 0;
            }

            if (heavyAttackComboIndex == 0)
            {
                animator.CrossFade(hashHeavyAttack1, 0.05f);
            }
            else if (heavyAttackComboIndex == 1)
            {
                animator.CrossFade(hashHeavyAttack2, 0.05f);
            }
            else
            {
                animator.CrossFade(hashHeavyAttack3, 0.05f);
            }

            animator.SetBool(hashCombatting, true);

            staminaStatManager.DecreaseStamina(Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.heavyAttackStaminaCost : unarmedHeavyAttackStaminaCost);

            heavyAttackComboIndex++;
            timeSinceLastHeavyAttack = 0f;
        }


        public bool CanLightAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            var staminaCost = Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.lightAttackStaminaCost : unarmedLightAttackStaminaCost;
            if (staminaStatManager.HasEnoughStaminaForAction(staminaCost) == false)
            {
                //BGMManager.instance.PlayInsufficientStamina();

                return false;
            }

            return true;
        }

        public bool CanHeavyAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            var staminaCost = Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.heavyAttackStaminaCost : unarmedHeavyAttackStaminaCost;
            if (staminaStatManager.HasEnoughStaminaForAction(staminaCost) == false)
            {
                return false;
            }

            return true;
        }

        public void EnableRootMotion()
        {
            animator.applyRootMotion = true;
        }


        bool CanAttack()
        {
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

            /*if (thirdPersonController.Grounded == false)
            {
                return false;
            }*/

            // If in dialogue
            if (uIDocumentDialogueWindow.isActiveAndEnabled)
            {
                return false;
            }

            if (playerParryManager.IsBlocking()) {
                return false;
            }

            return true;
        }

        public bool IsJumpAttacking()
        {
            return animator.GetBool(hashIsJumpAttacking);
        }

        public bool IsStartingJumpAttack()
        {
            return animator.GetBool(hashIsStartingJumpAttack);
        }

        private void OnDisable()
        {
            animator.SetBool(hashIsJumpAttacking, false);
            animator.SetBool(hashIsStartingJumpAttack, false);
            animator.SetBool(hashCombatting, false);
            isHeavyAttacking = false;
        }

    }
}
