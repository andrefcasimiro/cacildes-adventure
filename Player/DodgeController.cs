using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace AF
{
    public class DodgeController : MonoBehaviour
    {
        public readonly int hashRoll = Animator.StringToHash("Roll");
        public readonly int hashDodgeRight = Animator.StringToHash("Dodge Right");
        public readonly int hashDodgeLeft = Animator.StringToHash("Dodge Left");
        public readonly int hashBackStep = Animator.StringToHash("BackStep");
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

        private LockOnManager lockOnManager;

        [Header("Stamina")]
        public int dodgeCost = 15;

        float maxStartupDelay = 0.25f;
        float startupDelay = Mathf.Infinity;

        private void Awake()
        {
            lockOnManager = FindObjectOfType<LockOnManager>(true);
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

                        HandleDodge();
                    }

                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Wood" && IsDodging())
            {
                var destroyable = other.GetComponent<Destroyable>();
                if (destroyable != null)
                {
                    destroyable.DestroyObject(other.ClosestPointOnBounds(destroyable.transform.position));
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
                //BGMManager.instance.PlayInsufficientStamina();
                return false;
            }

            return true;
        }

        public bool IsDodging()
        {
            return animator.GetBool(hashIsDodging);
        }

        void HandleDodge()
        {
            if (lockOnManager.isLockedOn)
            {
                /*if (_input.move.x > 0)
                {
                    animator.CrossFade(hashDodgeRight, 0.05f);


                    return;
                }
                else if (_input.move.x < 0)
                {
                    animator.CrossFade(hashDodgeLeft, 0.05f);
                    return;
                }*/
            }


            if (_input.move == Vector2.zero)
            {
                animator.CrossFade(hashBackStep, 0.05f);
                return;
            }


            animator.CrossFade(hashRoll, 0.05f);
        }

    }

}
