using UnityEngine;

namespace AF
{

    public class DisableRootMotionOnEnter : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = false;
        }
    }

}
