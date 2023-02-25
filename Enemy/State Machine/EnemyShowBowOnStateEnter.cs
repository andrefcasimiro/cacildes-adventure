using UnityEngine;

namespace AF
{
    public class EnemyShowBowOnStateEnter : StateMachineBehaviour
    {
        EnemyManager enemyManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemyManager);

            if (enemyManager == null)
            {
                enemyManager = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (enemyManager.enemyProjectileController != null)
            {
                enemyManager.enemyProjectileController.ShowBow();
            }
        }

    }

}
