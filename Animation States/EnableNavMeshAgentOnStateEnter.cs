using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    // Confirm if is unused
    public class EnableNavMeshAgentOnStateEnter : StateMachineBehaviour
    {
        [Header("Companion / Enemy Hybrid")]
        public bool disableRootMotion = true;

        UnityEngine.AI.NavMeshAgent navMeshAgent;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (disableRootMotion)
            {
                animator.applyRootMotion = false;
            }

            if (navMeshAgent == null)
            {
                animator.TryGetComponent(out navMeshAgent);
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = animator.GetComponentInParent<NavMeshAgent>();
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.isStopped = false;
            }
        }
    }
}
