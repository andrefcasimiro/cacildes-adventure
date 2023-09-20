using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerState_Rolling : StateMachineBehaviour
    {

        DodgeController dodgeController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (dodgeController == null)
            {
                dodgeController = animator.GetComponent<DodgeController>();
            }

            if (dodgeController != null)
            {
                dodgeController.currentRequestForRollDuration = 0f;
            }
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            dodgeController.currentRequestForRollDuration += Time.deltaTime;
        }
    }

}
   