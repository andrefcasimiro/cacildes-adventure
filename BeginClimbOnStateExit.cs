using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class BeginClimbOnStateExit : StateMachineBehaviour
    {

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<ClimbController>().BeginClimbing();
        }
    }

}
