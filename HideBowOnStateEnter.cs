using UnityEngine;

namespace AF
{

    public class HideBowOnStateEnter : StateMachineBehaviour
    {

        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.HideBow();
        }

    }

}
