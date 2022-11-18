using UnityEngine;

namespace AF
{
    public class EnableRootMotion : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = true;
        }
    }
}