using UnityEngine;

namespace AF
{

    public class ActivateDeathColliderOnStateEnter : StateMachineBehaviour
    {
        EnemyManager enemyManager;
        Collider collider;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemyManager = animator.GetComponentInParent<EnemyManager>();
            if (enemyManager == null)
            {
                enemyManager = animator.GetComponent<EnemyManager>();
            }

            if (enemyManager != null)
            {
                var deathCollider = enemyManager.GetComponentInChildren<DeathColliderRef>(true);
                if (deathCollider == null)
                {
                    return;
                }

                collider = deathCollider.GetComponent<BoxCollider>();

                collider.enabled = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
}
