using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRotationOnStateEnter : StateMachineBehaviour
{
    StarterAssets.ThirdPersonController thirdPersonController;
    LockOnManager lockOnManager;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (thirdPersonController == null)
        {
            thirdPersonController = animator.GetComponent<StarterAssets.ThirdPersonController>();
        }

        if (lockOnManager == null)
        {
            lockOnManager = FindObjectOfType<LockOnManager>(true);
        }

        thirdPersonController.canRotateCharacter = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lockOnManager.nearestLockOnTarget != null && lockOnManager.isLockedOn)
        {
            var lookDirection = lockOnManager.nearestLockOnTarget.transform.position - animator.transform.position;
            lookDirection.y = 0;
            Quaternion lookRot = Quaternion.LookRotation(lookDirection);
            animator.transform.rotation = lookRot;
        }
    }
}
