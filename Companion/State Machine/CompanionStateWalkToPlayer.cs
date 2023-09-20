using UnityEngine;

namespace AF
{

    public class CompanionStateWalkToPlayer : StateMachineBehaviour
    {
        CompanionManager companionManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager == null)
            {
                animator.gameObject.TryGetComponent<CompanionManager>(out companionManager);

                companionManager = animator.GetComponentInParent<CompanionManager>(true);
            }
            companionManager.agent.isStopped = false;

            companionManager.agent.speed = companionManager.walkSpeed;
            companionManager.agent.stoppingDistance = companionManager.defaultStoppingDistance;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            companionManager.agent.isStopped = false;

            if (companionManager.ShouldRunToPlayer())
            {
                animator.SetBool(companionManager.hashRunToPlayer, true);
            }
            else if (companionManager.ShouldWalkToPlayer() == false)
            {
                animator.SetBool(companionManager.hashWalkToPlayer, false);
            }
            else if (!companionManager.agent.SetDestination(companionManager.player.transform.position))
            {
                companionManager.agent.ResetPath();
            }

        }

    }

}
