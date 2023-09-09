using UnityEngine;

namespace AF
{
    public class EnemyRecoverPostureOnStateExit : StateMachineBehaviour
    {
        EnemyManager enemyManager;


        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemyManager = animator.GetComponent<EnemyManager>();
            if (enemyManager == null)
            {
                enemyManager = animator.GetComponentInParent<EnemyManager>();
            }

            if (enemyManager != null && enemyManager.enemyPostureController != null)
            {
                enemyManager.enemyPostureController.RecoverPosture();
            }
        }
    }

}
