using System.Collections;
using AF.Ladders;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class DodgeController : MonoBehaviour
    {
        // Animation hash values
        public readonly int hashRoll = Animator.StringToHash("Roll");
        public readonly int hashBackStep = Animator.StringToHash("BackStep");

        [Header("Components")]
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;

        [Header("Stamina Settings")]
        public int dodgeCost = 15;

        [Header("In-game flags")]
        public bool isDodging = false;

        public float maxRequestForRollDuration = 0.4f;
        [HideInInspector] public float currentRequestForRollDuration = Mathf.Infinity;

        [Header("Dodge Attacks")]
        public int dodgeAttackBonus = 30;

        [Header("Unity Events")]
        public UnityEvent onDodge;

        public void ResetStates()
        {
            isDodging = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnDodgeInput()
        {
            if (CanDodge())
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
            isDodging = true;

            playerManager.PlayBusyHashedAnimationWithRootMotion(hashRoll);

            /*
            if (playerManager.equipmentGraphicsHandler.IsHeavyWeight())
            {
                StartCoroutine(StopHeavyRollRootmotion());
            }
            else if (playerManager.equipmentGraphicsHandler.IsMidWeight())
            {
                StartCoroutine(StopMidRollRootmotion());
            }*/
        }

        IEnumerator StopMidRollRootmotion()
        {
            yield return new WaitForSeconds(0.75f);
            playerManager.animator.applyRootMotion = false;
        }

        IEnumerator StopHeavyRollRootmotion()
        {
            yield return new WaitForSeconds(0.3f);
            isDodging = false;

            yield return new WaitForSeconds(0.3f);
            playerManager.animator.applyRootMotion = false;
        }

        public bool ShouldBackstep()
        {
            return playerManager.starterAssetsInputs.move == Vector2.zero && playerManager.thirdPersonController.skateRotation == false;
        }

        private bool CanDodge()
        {
            if (isDodging)
            {
                return false;
            }

            if (playerManager.IsBusy())
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
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
    }
}
