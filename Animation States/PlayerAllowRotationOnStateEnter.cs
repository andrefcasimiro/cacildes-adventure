using UnityEngine;

namespace AF
{
    public class PlayerAllowRotationOnStateEnter : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }

            playerManager.thirdPersonController.canRotateCharacter = true;
        }
    }
}
