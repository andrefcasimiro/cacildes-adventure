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
        public readonly int hashJumpAttack = Animator.StringToHash("Jump Attack");

        [Header("Transform Refs")]
        public Transform headRef;

        [Header("Sounds")]
        public AudioSource combatAudioSource;
        public AudioClip blockingSfx;

        [Header("Attack Combo Index")]
        [SerializeField] int attackComboIndex = 0;
        public float maxIdleCombo = 2f;
        float timeSinceLastAttack = 0f;

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

        bool canJumpAttack = true;

        public bool isParrying = false;

        [Header("Stamina")]
        public int unarmedLightAttackStaminaCost = 15;

        private void Start()
        {
            uIDocumentDialogueWindow = FindObjectOfType<UIDocumentDialogueWindow>(true);
            equipmentGraphicsHandler.DeactivateAllHitboxes();
        }

        private void Update()
        {
            // If has attacked on air, allow again if grounded
            if (canJumpAttack == false)
            {
                if (thirdPersonController.Grounded)
                {
                    canJumpAttack = true;
                }
            }

            isCombatting = animator.GetBool(hashCombatting);

            if (timeSinceLastAttack > maxIdleCombo)
            {
                attackComboIndex = 0;
                timeSinceLastAttack = 0;
            }
            else if (attackComboIndex != 0)
            {
                timeSinceLastAttack += Time.deltaTime;
            }

            if (starterAssetsInputs.lightAttack)
            {
                starterAssetsInputs.lightAttack = false;


                /*if (!thirdPersonController.Grounded && animator.GetBool(thirdPersonController._animIDFreeFall) && canJumpAttack)
                {
                    equipmentGraphicsHandler.ShowWeapons();
                    animator.CrossFade(hashJumpAttack, 0.05f);
                    canJumpAttack = false;
                    return;
                }*/

                if (CanLightAttack())
                {
                    HandleLightAttack();
                }
            }
        }

        public void HandleLightAttack()
        {
            if (isCombatting)
            {
                return;
            }

            equipmentGraphicsHandler.ShowWeapons();

            if (attackComboIndex > 2)
            {
                attackComboIndex = 0;
            }

            if (attackComboIndex == 0)
            {
                animator.CrossFade(hashLightAttack1, 0.05f);
            }
            else if (attackComboIndex == 1)
            {
                animator.CrossFade(hashLightAttack2, 0.05f);
            }
            else
            {
                animator.CrossFade(hashLightAttack3, 0.05f);
            }

            animator.SetBool(hashCombatting, true);

            staminaStatManager.DecreaseStamina(Player.instance.equippedWeapon != null ? Player.instance.equippedWeapon.lightAttackStaminaCost : unarmedLightAttackStaminaCost);

            attackComboIndex++;
            timeSinceLastAttack = 0f;
        }

        public void EnableRootMotion()
        {
            animator.applyRootMotion = true;
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
                return false;
            }

            return true;
        }

        bool CanAttack()
        {
            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return false;
            }

            if (dodgeController.IsDodging())
            {
                return false;
            }

            if (thirdPersonController.Grounded == false)
            {
                return false;
            }

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

    }
}
