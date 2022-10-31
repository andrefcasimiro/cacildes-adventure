using UnityEngine;

namespace AF
{
    public class StopMovement : StateMachineBehaviour
    {
        Enemy enemy;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
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
