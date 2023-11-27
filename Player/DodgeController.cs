using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AF
{
    public class DodgeController : MonoBehaviour
    {
        // Animation hash values
        public readonly int hashRoll = Animator.StringToHash("Roll");
        public readonly int hashBackStep = Animator.StringToHash("BackStep");

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Stamina Settings")]
        public int dodgeCost = 15;

        [Header("In-game flags")]
        public bool hasIframes = false;

        public float maxRequestForRollDuration = 0.4f;
        [HideInInspector] public float currentRequestForRollDuration = Mathf.Infinity;

        [Header("Dodge Attacks")]
        public int dodgeAttackBonus = 30;

        [Header("Unity Events")]
        public UnityEvent onDodge;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnDodgeInput()
        {
            if (CanDodge() && !IsDodging())
            {
                playerManager.staminaStatManager.DecreaseStamina(dodgeCost);
                playerManager.playerBlockInput.OnBlockInput_Cancelled();
                Tick();
                onDodge?.Invoke();
            }
        }

        void Tick()
        {
            if (ShouldBackstep())
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashBackStep);
                return;
            }

            HandleDodge();
        }

        void HandleDodge()
        {
            hasIframes = true;

            playerManager.PlayBusyHashedAnimationWithRootMotion(hashRoll);

            if (playerManager.equipmentGraphicsHandler.IsHeavyWeight())
            {
                StartCoroutine(StopHeavyRollRootmotion());
            }
            else if (playerManager.equipmentGraphicsHandler.IsMidWeight())
            {
                StartCoroutine(StopMidRollRootmotion());
            }
        }

        IEnumerator StopMidRollRootmotion()
        {
            yield return new WaitForSeconds(0.75f);
            playerManager.animator.applyRootMotion = false;
        }

        IEnumerator StopHeavyRollRootmotion()
        {
            yield return new WaitForSeconds(0.3f);
            StopIframes();

            yield return new WaitForSeconds(0.3f);
            playerManager.animator.applyRootMotion = false;
        }

        public bool ShouldBackstep()
        {
            return playerManager.starterAssetsInputs.move == Vector2.zero && playerManager.thirdPersonController.skateRotation == false;
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
                playerManager.starterAssetsInputs.move == Vector2.zero
                && playerManager.thirdPersonController.skateRotation == false
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
            if (playerManager.IsBusy())
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return false;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                return false;
            }

            if (!playerManager.thirdPersonController.Grounded || !playerManager.thirdPersonController.canMove)
            {
                return false;
            }

            if (!playerManager.staminaStatManager.HasEnoughStaminaForAction(dodgeCost))
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
