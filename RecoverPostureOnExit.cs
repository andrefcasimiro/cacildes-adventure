using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class RecoverPostureOnExit : StateMachineBehaviour
    {
        EnemyManager enemyManager;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemyManager = animator.GetComponent<EnemyManager>();
            if (enemyManager == null)
            {
                enemyManager = animator.GetComponentInParent<EnemyManager>();
            }

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemyManager.RecoverPosture();
        }
    }

}