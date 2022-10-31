using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class RecoverPostureOnExit : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EnemyPostureController enemyPostureController = animator.GetComponent<EnemyPostureController>();
            if (enemyPostureController == null)
            {
                enemyPostureController = animator.GetComponentInParent<EnemyPostureController>();
            }

            enemyPostureController.RecoverPosture();
        }
    }

}