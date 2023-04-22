using UnityEngine;

namespace AF
{

    public class CompanionStateIdle : StateMachineBehaviour
    {
        CompanionManager companionManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager == null)
            {
                animator.gameObject.TryGetComponent<CompanionManager>(out companionManager);

                companionManager = animator.GetComponentInParent<CompanionManager>(true);
            }

            if (companionManager.agent != null && companionManager.gameObject.activeSelf && companionManager.agent.isOnNavMesh)
            {
                companionManager.agent.isStopped = false;

                companionManager.agent.stoppingDistance = companionManager.defaultStoppingDistance;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager.waitingForPlayer || companionManager.inParty == false)
            {
                return;
            }

            if (companionManager.ShouldRunToPlayer())
            {
                animator.SetBool(companionManager.hashRunToPlayer, true);
            }
            else if (companionManager.ShouldWalkToPlayer())
            {
                animator.SetBool(companionManager.hashWalkToPlayer, true);
            }
        }

    }

}
