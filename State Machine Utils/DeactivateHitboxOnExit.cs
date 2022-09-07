using UnityEngine;

namespace AF
{

    public class DeactivateHitboxOnExit : StateMachineBehaviour
    {
        ICombatable combatManager;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<ICombatable>(out combatManager);

            if (combatManager == null)
            {
                combatManager = animator.GetComponentInParent<ICombatable>(true);
            }

            if (combatManager != null)
            {
                combatManager.DeactivateHitbox();
            }
        }
    }

}
