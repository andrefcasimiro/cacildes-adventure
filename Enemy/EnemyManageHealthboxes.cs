using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyManageHealthboxes : StateMachineBehaviour
    {
        EnemyManager enemyManager;

        public bool activateHealthHitboxes = false;
        public bool deactivateHealthHitboxes = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent<EnemyManager>(out enemyManager);

            if (enemyManager == null)
            {
                enemyManager = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (activateHealthHitboxes)
            {
                enemyManager.enemyHealthController.EnableHealthHitboxes();
            }
            else if (deactivateHealthHitboxes)
            {
                    enemyManager.enemyHealthController.DisableHealthHitboxes();
            }
        }
    }

}
