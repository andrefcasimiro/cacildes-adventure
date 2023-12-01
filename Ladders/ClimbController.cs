
using System.Collections;
using UnityEngine;

namespace AF.Ladders
{
    public class ClimbController : MonoBehaviour
    {
        public readonly int hashStartFromTop = Animator.StringToHash("StartFromTop");
        public readonly int hashStartFromBottom = Animator.StringToHash("StartFromBottom");
        public readonly int hashExitToTop = Animator.StringToHash("ExitToTop");
        public readonly int hashExitToBottom = Animator.StringToHash("ExitToBottom");
        public readonly string hashClimbAmount = "climbAmount";

        [Header("Climbing State")]
        public ClimbState climbState = ClimbState.NONE;

        [Header("Components")]
        public PlayerManager playerManager;

        public StarterAssetsInputs inputs;

        [Header("Settings")]
        public float EXIT_TO_TOP_RECOVER_CONTROL_DELAY = 2f;
        public float EXIT_TO_BOTTOM_RECOVER_CONTROL_DELAY = 1.2f;

        public void StartClimbing(Transform ladderTransform, bool isFromBottom)
        {
            playerManager.playerWeaponsManager.HideEquipment();
            playerManager.playerComponentManager.DisableCharacterController();
            playerManager.thirdPersonController.canRotateCharacter = false;

            climbState = ClimbState.ENTERING;

            if (isFromBottom)
            {
                StartFromBottom(ladderTransform);
            }
            else
            {
                StartFromTop(ladderTransform);
            }
        }

        void TeleportToLadder(Transform ladderTransform)
        {
            playerManager.playerComponentManager.DisableCharacterController();

            Vector3 teleportTargetWorldPos = ladderTransform.TransformPoint(Vector3.zero);

            // Teleport the player to the target's position
            playerManager.transform.position = teleportTargetWorldPos;

            playerManager.transform.rotation = ladderTransform.transform.rotation;

            playerManager.playerComponentManager.EnableCharacterController();
        }

        public void StartFromBottom(Transform ladderTransform)
        {
            TeleportToLadder(ladderTransform);
            playerManager.PlayBusyHashedAnimationWithRootMotion(hashStartFromBottom);

            StartCoroutine(RecoverControl_Coroutine(true, EXIT_TO_BOTTOM_RECOVER_CONTROL_DELAY));
        }

        public void StartFromTop(Transform ladderTransform)
        {
            TeleportToLadder(ladderTransform);
            playerManager.PlayBusyHashedAnimationWithRootMotion(hashStartFromTop);
            StartCoroutine(RecoverControl_Coroutine(true, EXIT_TO_TOP_RECOVER_CONTROL_DELAY));
        }

        public void ExitToTop()
        {
            climbState = ClimbState.EXITING;
            playerManager.PlayBusyHashedAnimationWithRootMotion(hashExitToTop);
            StartCoroutine(RecoverControl_Coroutine(false, EXIT_TO_TOP_RECOVER_CONTROL_DELAY));
        }

        public void ExitToBottom()
        {
            climbState = ClimbState.EXITING;
            playerManager.PlayBusyHashedAnimationWithRootMotion(hashExitToBottom);
            StartCoroutine(RecoverControl_Coroutine(false, EXIT_TO_BOTTOM_RECOVER_CONTROL_DELAY));
        }

        IEnumerator RecoverControl_Coroutine(bool isClimbing, float delay)
        {
            playerManager.playerComponentManager.DisableCharacterController();

            yield return new WaitForSeconds(delay);

            playerManager.playerComponentManager.EnableCharacterController();

            if (isClimbing)
            {
                Physics.gravity = new Vector3(0, 0, 0);
                playerManager.animator.applyRootMotion = true;
                climbState = ClimbState.CLIMBING;
            }
            else
            {
                Physics.gravity = new Vector3(0, -9.81f, 0);
                climbState = ClimbState.NONE;
                playerManager.animator.applyRootMotion = false;
                playerManager.playerWeaponsManager.ShowEquipment();
                playerManager.thirdPersonController.canRotateCharacter = true;
            }

        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnMoveInput()
        {
            if (climbState != ClimbState.CLIMBING)
            {
                return;
            }

            Climb(inputs.move.y);
        }

        public void Climb(float direction)
        {

            if (direction < 0)
            {
                direction = -1;
            }
            else if (direction > 0)
            {
                direction = 1;
            }

            playerManager.animator.SetFloat(hashClimbAmount, direction);
        }
    }
}
