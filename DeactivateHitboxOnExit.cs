using UnityEngine;

namespace AF
{

    public class DeactivateHitboxOnExit : StateMachineBehaviour
    {
        Enemy enemy;
        EnemyCombatController enemyCombatController;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyCombatController == null)
            {
                enemyCombatController = animator.GetComponent<EnemyCombatController>();
            }

            if (enemyCombatController != null)
            {
                enemyCombatController.DeactivateAreaOfImpactHitbox();
                enemyCombatController.DeactivateHeadHitbox();
                enemyCombatController.DeactivateLeftHandHitbox();
                enemyCombatController.DeactivateLeftLegHitbox();
                enemyCombatController.DeactivateRightHandHitbox();
                enemyCombatController.DeactivateRightLegHitbox();
            }
        }
    }

}
