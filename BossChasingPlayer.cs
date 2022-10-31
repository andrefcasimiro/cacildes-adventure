using UnityEngine;

namespace AF
{

    public class BossChasingPlayer : StateMachineBehaviour
    {

        Enemy enemy;
        EnemyPathController enemyPathController;
        EnemyProjectileController enemyProjectileController;
        EnemyCombatController enemyCombatController;

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

            if (enemyProjectileController == null)
            {
                enemyProjectileController = enemy.GetComponent<EnemyProjectileController>();
            }

            if (enemyCombatController == null)
            {
                enemyCombatController = enemy.GetComponent<EnemyCombatController>();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.agent.SetDestination(enemy.playerCombatController.transform.position);

            if (enemyProjectileController.CanShoot())
            {
                enemyProjectileController.PrepareProjectile();
                return;
            }

            float distanceBetweenEnemyAndTarget = Vector3.Distance(enemy.agent.transform.position, enemy.playerCombatController.transform.position);
            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemyPathController.hashPatrol, true);
            }
            else if (distanceBetweenEnemyAndTarget <= enemy.agent.stoppingDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemyCombatController.hashCombatting, true);
            }

        }

    }

}