using UnityEngine;

namespace AF
{

    public class StopAgent : StateMachineBehaviour
    {
        EnemyManager enemy;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.agent.isStopped = true;
        }

    }

}
