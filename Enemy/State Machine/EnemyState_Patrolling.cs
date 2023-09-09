using UnityEngine;

namespace AF
{

    public class EnemyState_Patrolling : StateMachineBehaviour
    {
        EnemyManager enemy;

        public float walkSpeed = -1;

        float originalSpeed;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);
            
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.RepositionNavmeshAgent();

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


            if (walkSpeed > 0)
            {
                originalSpeed = enemy.agent.speed;
                enemy.agent.speed = walkSpeed;
            }

            if (enemy.walkSpeedOverride != -1)
            {
                originalSpeed = enemy.agent.speed;

                enemy.agent.speed = enemy.walkSpeedOverride;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.eventPage != null && enemy.eventPage.isRunning)
            {
                // enemy.agent.isStopped = true;
                animator.SetBool(enemy.hashPatrol, false);
                return;
            }
            else
            {
                enemy.agent.isStopped = false;
            }

            if (enemy.enemySightController.IsPlayerInSight())
            {
                // enemy.agent.isStopped = true;
                animator.SetBool(enemy.hashPatrol, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            if (!enemy.agent.pathPending && enemy.agent.remainingDistance < enemy.agent.stoppingDistance + 0.5f)
            {
                animator.SetBool(enemy.hashPatrol, false);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (walkSpeed != 0 && originalSpeed != 0)
            {
                enemy.agent.speed = originalSpeed;
            }
        }
    }
}
