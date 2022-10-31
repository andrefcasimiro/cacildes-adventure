using UnityEngine;

namespace AF
{

    public class EnemyStateIdle : StateMachineBehaviour
    {
        Enemy enemy;
        EnemyPathController enemyPathController;
        EnemySightController enemySightController;

        float currentTimeOnWaypoint;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currentTimeOnWaypoint = 0;

            if (enemy == null)
            {
                animator.gameObject.TryGetComponent<Enemy>(out enemy);

                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyPathController == null)
            {
                enemyPathController = enemy.GetComponent<EnemyPathController>();
            }

            if (enemySightController == null)
            {
                enemySightController = enemy.GetComponent<EnemySightController>();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            if (enemySightController.IsPlayerInSight() == false)
            {
                currentTimeOnWaypoint += Time.deltaTime;

                if (currentTimeOnWaypoint > enemyPathController.restingTimeOnWaypoint)
                {
                    animator.SetBool(enemyPathController.hashPatrol, true);
                }
            }
            else
            {
                animator.SetBool(enemy.hashChasing, true);
            }
        }

    }

}
