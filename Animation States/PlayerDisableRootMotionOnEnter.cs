using UnityEngine;

namespace AF
{
    public class PlayerDisableRootMotionOnEnter : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = false;
        }
    }
}
