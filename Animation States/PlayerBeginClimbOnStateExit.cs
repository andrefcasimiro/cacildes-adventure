using UnityEngine;

namespace AF
{
    public class PlayerBeginClimbOnStateExit : StateMachineBehaviour
    {
        ClimbController climbController;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (climbController == null)
            {
                climbController = animator.GetComponent<ClimbController>();
            }

            climbController.BeginClimbing();
        }
    }
}
