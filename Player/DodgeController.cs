using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Animations;

namespace AF
{
    public class DodgeController : MonoBehaviour
    {
        public readonly int hashRoll = Animator.StringToHash("Roll");
        public readonly int hashCrouchRoll = Animator.StringToHash("Dodge Crouch");
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

        public bool hasIframes = false;

        [HideInInspector]
        public bool canRequestRollAttack = true;
        public float maxRequestForRollDuration = 0.4f;
        public float currentRequestForRollDuration = Mathf.Infinity;

        CharacterController characterController => GetComponent<CharacterController>();

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
            if (!IsDodging())
            {
                return;
            }

            if (other.gameObject.CompareTag("Wood"))
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

            if (!thirdPersonController.canMove)
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

        public bool IsRollAttacking()
        {
            return animator.GetBool("IsRollAttacking");
        }

        public bool CanRollAttack()
        {
            if (IsRollAttacking())
            {
                return false;
            }

            // Is Backstepping, give slight offset
            if (_input.move == Vector2.zero && thirdPersonController.skateRotation == false && currentRequestForRollDuration > maxRequestForRollDuration - 0.1f)
            {
                return false;
            }

            if (currentRequestForRollDuration > maxRequestForRollDuration)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void StopIframes()
        {
            hasIframes = false;
        }

        void HandleDodge()
        {
            if (_input.move == Vector2.zero && thirdPersonController.skateRotation == false)
            {
                animator.CrossFade(hashBackStep, 0.05f);
                return;
            }

            hasIframes = true;

            if (thirdPersonController.skateRotation)
            {
                animator.Play(hashCrouchRoll);
            }
            else
            {
                animator.CrossFade(hashRoll, 0.05f);

                if (equipmentGraphicsHandler.IsHeavyWeight())
                {
                    StartCoroutine(StopHeavyRollRootmotion());
                }
                else if (equipmentGraphicsHandler.IsMidWeight())
                {
                    StartCoroutine(StopMidRollRootmotion());
                }
            }
        }

        IEnumerator StopMidRollRootmotion()
        {
            yield return new WaitForSeconds(0.75f);
            animator.applyRootMotion = false;
        }

        IEnumerator StopHeavyRollRootmotion()
        {
            yield return new WaitForSeconds(0.3f);
            StopIframes();

            yield return new WaitForSeconds(0.3f);
            animator.applyRootMotion = false;
        }


    }

}
