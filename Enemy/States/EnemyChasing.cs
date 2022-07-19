using UnityEngine;

namespace AF
{

    public class EnemyChasing : StateMachineBehaviour
    {
        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.player.IsNotAvailable())
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashPatrol, true);
                return;
            }

            Utils.FaceTarget(enemy.transform, enemy.player.transform);
            enemy.agent.SetDestination(enemy.player.transform.position);

            float distanceBetweenEnemyAndTarget = Vector3.Distance(enemy.agent.transform.position, enemy.player.transform.position);

            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashPatrol, true);
            }
            else if (distanceBetweenEnemyAndTarget <= enemy.agent.stoppingDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashCombatting, true);
            }

        }

    }

}