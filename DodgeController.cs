using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace AF
{
    public class DodgeController : MonoBehaviour
    {
        public readonly int hashDodge = Animator.StringToHash("Dodge");
        public readonly int hashIsDodging = Animator.StringToHash("IsDodging");

        private Animator animator => GetComponent<Animator>();
        private StarterAssetsInputs _input => GetComponent<StarterAssetsInputs>();
        private ClimbController climbController => GetComponent<ClimbController>();
        private PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        private ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();
        private FootstepListener footstepListener => GetComponent<FootstepListener>();
        private EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();
        private StaminaStatManager staminaStatManager => GetComponent<StaminaStatManager>();

        private MenuManager menuManager;

        [Header("Stamina")]
        public int dodgeCost = 15;


        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void Update()
        {

            if (_input.dodge)
            {
                _input.dodge = false;

                if (CanDodge())
                {
                    if (!IsDodging())
                    {
                        staminaStatManager.DecreaseStamina(dodgeCost);
                        animator.CrossFade(hashDodge, 0.05f);
                    }

                }
            }
        }

        private bool CanDodge()
        {

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return false;
            }

            if (playerCombatController.isCombatting)
            {
                return false;
            }

            if (!thirdPersonController.Grounded)
            {
                return false;
            }

            if (!staminaStatManager.HasEnoughStaminaForAction(dodgeCost))
            {
                return false;
            }

            return true;
        }

        public bool IsDodging()
        {
            return animator.GetBool(hashIsDodging);
        }

    }

}
