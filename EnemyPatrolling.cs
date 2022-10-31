using UnityEngine;

namespace AF
{

    public class EnemyPatrolling : StateMachineBehaviour
    {
        Enemy enemy;
        EnemyPathController enemyPathController;
        EnemySightController enemySightController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            
            if (enemy == null)
            {
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

            enemyPathController.GotoNextPoint();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemySightController.IsPlayerInSight())
            {
                enemy.agent.isStopped = true;
                animator.SetBool(enemyPathController.hashPatrol, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            if (!enemy.agent.pathPending && enemy.agent.remainingDistance < enemy.agent.stoppingDistance + 0.5f)
            {
                animator.SetBool(enemyPathController.hashPatrol, false);
            }
        }

    }

}
