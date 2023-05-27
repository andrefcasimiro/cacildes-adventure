using UnityEngine;

namespace AF
{
    /// <summary>
    /// Applies only for cases where enemy shield is not always visible
    /// </summary>
    public class Enemy_HandleShieldVisibilityOnStateEnter : StateMachineBehaviour
    {
        EnemyBlockController enemyBlockController;

        public bool shouldHide = false;
        public bool shouldShow = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemyBlockController == null)
            {
                animator.TryGetComponent(out enemyBlockController);
            }

            if (enemyBlockController == null)
            {
                enemyBlockController = animator.GetComponentInParent<EnemyBlockController>(true);
            }

            if (enemyBlockController != null && enemyBlockController.isShieldAlwaysVisible == false && enemyBlockController.shield != null)
            {
                if (shouldHide)
                {
                    enemyBlockController.shield.gameObject.SetActive(false);
                }
                else if (shouldShow)
                {
                    enemyBlockController.shield.gameObject.SetActive(true);
                }
            }
        }
    }

}
