using UnityEngine;

namespace AF
{

    public class EnemyState_Patrolling : StateMachineBehaviour
    {
        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);
            
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (enemy.enemyPatrolController == null || enemy.enemyPatrolController.waypoints.Count < 0)
            {
                animator.SetBool(enemy.hashPatrol, false);
                return;
            }

            if (enemy.alwaysTrackPlayer == true)
            {
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            enemy.enemyPatrolController.GotoNextPoint();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.eventPage != null && enemy.eventPage.IsRunning())
            {
                enemy.agent.isStopped = true;
                animator.SetBool(enemy.hashPatrol, false);
                return;
            }
            else
            {
                enemy.agent.isStopped = false;
            }

            if (enemy.enemySightController.IsPlayerInSight())
            {
                enemy.agent.isStopped = true;
                animator.SetBool(enemy.hashPatrol, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            if (!enemy.agent.pathPending && enemy.agent.remainingDistance < enemy.agent.stoppingDistance + 0.5f)
            {
                animator.SetBool(enemy.hashPatrol, false);
            }
        }
    }
}
