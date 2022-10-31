using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyWaiting : StateMachineBehaviour
    {        
        Enemy enemy;
        EnemyCombatController enemyCombatController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyCombatController == null)
            {
                enemyCombatController = enemy.GetComponent<EnemyCombatController>();
            }

            float turnChance = Random.Range(enemyCombatController.minWaitingTimeBeforeResumingCombat, enemyCombatController.maxWaitingTimeBeforeResumingCombat);
            enemyCombatController.turnWaitingTime = turnChance;
        }

    }

}
