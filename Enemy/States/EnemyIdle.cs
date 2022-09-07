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

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            if (
                enemy.player.IsBusy() == false
                && Utils.PlayerIsSighted(enemy, enemy.player, enemy.obstructionMask)
            )
            {
                animator.SetBool(enemy.hashPatrol, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            currentTimeOnWaypoint += Time.deltaTime;

            if (currentTimeOnWaypoint > enemy.restingTimeOnWaypoint)
            {
                animator.SetBool(enemy.hashPatrol, true);
            }
        }

    }

}