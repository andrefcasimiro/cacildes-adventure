using UnityEngine;

namespace AF
{

    public class EnemyStateIdle : StateMachineBehaviour
    {
        EnemyManager enemy;

        float currentTimeOnWaypoint;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currentTimeOnWaypoint = 0;

            if (enemy == null)
            {
                animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            if (enemy.IsPlayerInSight() == false)
            {
                currentTimeOnWaypoint += Time.deltaTime;

                if (currentTimeOnWaypoint > enemy.restingTimeOnWaypoint && enemy.waypoints.Count > 0)
                {
                    animator.SetBool(enemy.hashPatrol, true);
                }
            }
            else
            {
                animator.SetBool(enemy.hashChasing, true);
            }
        }

    }

}
