using UnityEngine;

namespace AF
{

    public class EnemyIdle : StateMachineBehaviour
    {
        Enemy enemy;
        float currentTimeOnWaypoint;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            currentTimeOnWaypoint = 0;
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            currentTimeOnWaypoint += Time.deltaTime;

            if (currentTimeOnWaypoint > enemy.restingTimeOnWaypoint)
            {
                animator.SetBool(enemy.hashPatrol, true);
            }
        }

    }

}