using UnityEngine;

namespace AF
{
    public class PlayerAllowRotationOnStateEnter : StateMachineBehaviour
    {
        StarterAssets.ThirdPersonController thirdPersonController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (thirdPersonController == null)
            {
                thirdPersonController = animator.GetComponent<StarterAssets.ThirdPersonController>();
            }

            thirdPersonController.canRotateCharacter = true;
        }
    }
}
