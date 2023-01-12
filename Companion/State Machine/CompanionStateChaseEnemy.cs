using UnityEngine;

namespace AF
{

    public class CompanionStateChaseEnemy : StateMachineBehaviour
    {
        CompanionManager companionManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager == null)
            {
                animator.gameObject.TryGetComponent<CompanionManager>(out companionManager);

                companionManager = animator.GetComponentInParent<CompanionManager>(true);
            }

            companionManager.agent.speed = companionManager.runSpeed;

            companionManager.HideGenericTrigger();

            companionManager.agent.isStopped = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager.currentEnemy == null)
            {
                animator.SetBool(companionManager.hashIsChasingEnemy, false);
                return;
            }

            if (
                companionManager.IsEnemyFarAway() == false)
            {
                animator.SetBool(companionManager.hashIsCombatting, true);
                animator.SetBool(companionManager.hashIsChasingEnemy, false);
                return;
            }

            companionManager.agent.SetDestination(companionManager.currentEnemy.transform.position);
        }

    }

}
