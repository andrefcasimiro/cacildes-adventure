using UnityEngine;

namespace AF
{

    public class EnemyPatrolling : StateMachineBehaviour
    {
        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            enemy.GotoNextPoint();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Chase Player if available
            if (
                enemy.player.IsNotAvailable() == false
                && Utils.PlayerIsSighted(enemy.transform, enemy.player, enemy.obstructionMask)
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