using UnityEngine;

namespace AF
{
    public class StopMovement : StateMachineBehaviour
    {
        EnemyManager enemy;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.agent.velocity = Vector3.zero;
            enemy.agent.SetDestination(enemy.transform.position);
            enemy.agent.isStopped = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.agent.isStopped = false;
        }
    }
}
