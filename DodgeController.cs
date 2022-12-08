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
        PlayerShootingManager playerShootingManager => GetComponent<PlayerShootingManager>();

        private MenuManager menuManager;

        [Header("Stamina")]
        public int dodgeCost = 15;

        float maxStartupDelay = 0.25f;
        float startupDelay = Mathf.Infinity;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            startupDelay = 0f;
        }
        
        private void Update()
        {
            if (startupDelay < maxStartupDelay)
            {
                startupDelay += Time.deltaTime;
            }

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
            if (playerShootingManager.IsShooting())
            {
                return false;
            }

            if (startupDelay < maxStartupDelay)
            {
                return false;
            }

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
