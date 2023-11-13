using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{
    public class DodgeController : MonoBehaviour
    {
        // Animation hash values
        public readonly int hashRoll = Animator.StringToHash("Roll");
        public readonly int hashCrouchRoll = Animator.StringToHash("Dodge Crouch");
        public readonly int hashBackStep = Animator.StringToHash("BackStep");
        public readonly int hashIsDodging = Animator.StringToHash("IsDodging");

        [Header("Components")]
        public Animator animator;
        public StarterAssetsInputs starterAssetsInputs;
        public ClimbController climbController;
        public PlayerCombatController playerCombatController;
        public ThirdPersonController thirdPersonController;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public StaminaStatManager staminaStatManager;
        public PlayerShootingManager playerShootingManager;

        [Header("Stamina Settings")]
        public int dodgeCost = 15;

        [Header("In-game flags")]
        public bool hasIframes = false;

        public float maxRequestForRollDuration = 0.4f;
        [HideInInspector] public float currentRequestForRollDuration = Mathf.Infinity;

        [Header("Dodge Attacks")]
        public int dodgeAttackBonus = 30;

        #region Input System
        public void OnDodge(InputValue inputValue)
        {
            if (inputValue.isPressed && CanDodge() && !IsDodging())
            {
                staminaStatManager.DecreaseStamina(dodgeCost);
                Tick();
            }
        }
        #endregion

        #region Handlers
        void Tick()
        {
            if (ShouldBackstep())
            {
                animator.Play(hashBackStep);
                return;
            }

            HandleDodge();
        }

        void HandleDodge()
        {
            hasIframes = true;

            if (thirdPersonController.skateRotation)
            {
                animator.Play(hashCrouchRoll);
                return;
            }

            animator.Play(hashRoll);

            if (equipmentGraphicsHandler.IsHeavyWeight())
            {
                StartCoroutine(StopHeavyRollRootmotion());
            }
            else if (equipmentGraphicsHandler.IsMidWeight())
            {
                StartCoroutine(StopMidRollRootmotion());
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
        #endregion

        #region Booleans
        public bool ShouldBackstep()
        {
            return starterAssetsInputs.move == Vector2.zero && thirdPersonController.skateRotation == false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsDodging())
            {
                return;
            }

            if (other.CompareTag("Wood") && other.TryGetComponent<Destroyable>(out var destroyable))
            {
                destroyable.DestroyObject(other.ClosestPointOnBounds(destroyable.transform.position));
            }
        }

        public bool CanRollAttack()
        {
            if (IsRollAttacking())
            {
                return false;
            }

            if (
                starterAssetsInputs.move == Vector2.zero
                && thirdPersonController.skateRotation == false
                // Is Backstepping, give slight offset
                && currentRequestForRollDuration > maxRequestForRollDuration - 0.1f)
            {
                return false;
            }

            if (currentRequestForRollDuration > maxRequestForRollDuration)
            {
                return false;
            }

            return true;
        }

        private bool CanDodge()
        {
            if (playerShootingManager.IsShooting())
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

            if (!thirdPersonController.Grounded || !thirdPersonController.canMove)
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
            return false;
            ///            return animator.GetBool(hashIsDodging);
        }

        public bool IsRollAttacking()
        {
            return false;
            //            return animator.GetBool("IsRollAttacking");
        }
        #endregion


        #region Animation Events
        /// <summary>
        /// Animation Event
        /// </summary>
        public void StopIframes()
        {
            hasIframes = false;
        }
        #endregion
    }
}
