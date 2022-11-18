using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableComponentsOnStateExit : StateMachineBehaviour
{
    PlayerComponentManager playerComponentManager;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerComponentManager == null)
        {
            animator.GetComponent<PlayerComponentManager>();
        }
        else
        {
            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();
        }
    }
}
