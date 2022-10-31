using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class FinishClimb : StateMachineBehaviour
    {
        ClimbController climbController;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.TryGetComponent(out climbController);

            if (climbController == null)
            {
                Debug.LogError("Could not find ClimbController");
                return;
            }

            climbController.FinishClimbing();
        }
    }

}