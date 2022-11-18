using UnityEngine;

namespace AF
{

    public class EnemyChasing : StateMachineBehaviour
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

            if (enemy.isBoss)
            {
                if (enemy.bossMusic != null)
                {
                    BGMManager.instance.PlayMusic(enemy.bossMusic);
                }

                enemy.InitiateBossBattle();
            }
            else
            {
                BGMManager.instance.PlayBattleMusic();
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

            enemy.agent.isStopped = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.agent.enabled)
            {
                enemy.agent.SetDestination(enemy.playerCombatController.transform.position);
            }

            float distanceBetweenEnemyAndTarget = Vector3.Distance(enemy.agent.transform.position, enemy.playerCombatController.transform.position);

            // Evaluate if we can shoot
            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance / 2 && distanceBetweenEnemyAndTarget <= enemy.maximumChaseDistance)
            {
                if (enemyProjectileController.isReadyToShoot == false)
                {
                    enemyProjectileController.isReadyToShoot = true;
                }
                else if (enemyProjectileController.CanShoot())
                {
                    enemyProjectileController.PrepareProjectile();
                    return;
                }
            }

            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemyPathController.hashPatrol, true);

                BGMManager.instance.PlayMapMusicAfterKillingEnemy(enemy);

                enemyProjectileController.isReadyToShoot = false;
            }
            else if (distanceBetweenEnemyAndTarget <= enemy.agent.stoppingDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemyCombatController.hashCombatting, true);
            }

        }

    }

}
