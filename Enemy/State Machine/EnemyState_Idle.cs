using UnityEngine;

namespace AF
{

    public class EnemyState_Idle : StateMachineBehaviour
    {
        EnemyManager enemy;

        float currentTimeOnWaypoint;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currentTimeOnWaypoint = 0;

            if (enemy == null)
            {
                animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

                if (enemy == null)
                {
                    enemy = animator.GetComponentInParent<EnemyManager>(true);
                }
            }


            if (enemy.enemyHealthController != null)
            {
                enemy.enemyHealthController.HideHUD();
            }

            enemy.RepositionNavmeshAgent();

            enemy.agent.SetDestination(enemy.transform.position);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.eventPage != null && enemy.eventPage.isRunning)
            {
                // enemy.agent.isStopped = true;
                return;
            }

            if (enemy.enemySightController.IsPlayerInSight())
            {
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            if (enemy.enemyPatrolController != null)
            {
                currentTimeOnWaypoint += Time.deltaTime;

                if (currentTimeOnWaypoint > enemy.enemyPatrolController.restingTimeOnWaypoint && enemy.enemyPatrolController.waypoints.Count > 0)
                {
                    animator.SetBool(enemy.hashPatrol, true);
                }
            }
        }
    }
}
