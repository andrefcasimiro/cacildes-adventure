using AF;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_AllowComponentsOnStateExit : StateMachineBehaviour
{
    PlayerComponentManager playerComponentManager = null;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerComponentManager == null)
        {
            playerComponentManager = animator.GetComponent<PlayerComponentManager>();
        }

        if (playerComponentManager == null)
        {
            return;
        }

        playerComponentManager.EnableCharacterController();
        playerComponentManager.EnableComponents();
        playerComponentManager.GetComponent<ThirdPersonController>().canMove = true;
        playerComponentManager.GetComponent<ThirdPersonController>().canRotateCharacter = true;
    }
}
