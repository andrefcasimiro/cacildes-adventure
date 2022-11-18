using UnityEngine;

namespace AF
{

    public class ShowBowOnStateEnter : StateMachineBehaviour
    {

        Enemy enemy;
        EnemyProjectileController enemyProjectileController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyProjectileController == null)
            {
                enemyProjectileController = enemy.GetComponent<EnemyProjectileController>();
            }

            if (enemyProjectileController != null)
            {
                enemyProjectileController.ShowBow();
            }

            enemy.agent.isStopped = true;

        }

    }

}
