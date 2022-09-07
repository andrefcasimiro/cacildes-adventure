using UnityEngine;

namespace AF
{

    public class EnemyPatrolling : StateMachineBehaviour
    {
        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            enemy.GotoNextPoint();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Player Sighted: " + Utils.PlayerIsSighted(enemy, enemy.player, enemy.obstructionMask));

            // Chase Player if available
            if (
                enemy.player.IsBusy() == false
                && Utils.PlayerIsSighted(enemy, enemy.player, enemy.obstructionMask)
            )
            {
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