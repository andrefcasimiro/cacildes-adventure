using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowRotationOnStateEnter : StateMachineBehaviour
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
