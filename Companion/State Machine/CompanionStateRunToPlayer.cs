using UnityEngine;

namespace AF
{

    public class CompanionStateRunToPlayer : StateMachineBehaviour
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

            companionManager.agent.speed = companionManager.runSpeed;
            companionManager.agent.stoppingDistance = companionManager.defaultStoppingDistance;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            companionManager.agent.isStopped = false;

            if (companionManager.ShouldRunToPlayer() == false && companionManager.ShouldWalkToPlayer())
            {
                animator.SetBool(companionManager.hashWalkToPlayer, true);
                animator.SetBool(companionManager.hashRunToPlayer, false);
            }
            else if (companionManager.ShouldRunToPlayer() == false && companionManager.ShouldWalkToPlayer() == false)
            {
                animator.SetBool(companionManager.hashWalkToPlayer, false);
                animator.SetBool(companionManager.hashRunToPlayer, false);
            }
            else
            {
                if (!companionManager.agent.SetDestination(companionManager.player.transform.position))
                {
                    companionManager.agent.ResetPath();
                }
            }
        }

    }

}
