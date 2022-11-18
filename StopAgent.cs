using UnityEngine;

namespace AF
{

    public class StopAgent : StateMachineBehaviour
    {
        Enemy enemy;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            enemy.agent.isStopped = true;
        }

    }

}
