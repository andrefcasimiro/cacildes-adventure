using UnityEngine;

namespace AF
{

    public class EnemyStopAgentOnStateEnter : StateMachineBehaviour
    {
        EnemyManager enemy;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (enemy.IsNavMeshAgentActive())
            {
                // enemy.agent.isStopped = true;
            }
        }

    }

}
