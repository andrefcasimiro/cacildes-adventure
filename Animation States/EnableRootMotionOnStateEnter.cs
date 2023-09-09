using UnityEngine;

namespace AF
{
    public class EnableRootMotionOnStateEnter : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = true;
        }
    }
}
