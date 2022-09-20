using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyWaiting : StateMachineBehaviour
    {        
        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }


            float turnChance = Random.Range(0f, enemy.maxWaitingTimeBeforeResumingCombat);
            enemy.turnWaitingTime = turnChance;
            enemy.turnWaitingTime = turnChance;
            enemy.isWaiting = true;
            animator.SetBool(enemy.hashCombatting, false);
        }

    }

}
