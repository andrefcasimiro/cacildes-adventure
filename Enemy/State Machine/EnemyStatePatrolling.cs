using UnityEngine;

namespace AF
{

    public class EnemyStatePatrolling : StateMachineBehaviour
    {
        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);
            
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (enemy.waypoints.Count < 0)
            {
                enemy.agent.SetDestination(enemy.agent.transform.position);
                animator.SetBool(enemy.hashPatrol, false);
                return;
            }

            enemy.GotoNextPoint();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.IsPlayerInSight())
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
