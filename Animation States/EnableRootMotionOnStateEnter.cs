using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EnableRootMotionOnStateEnter : StateMachineBehaviour
    {
        [Header("Companion / Enemy Hybrid")]
        public bool disableNavMeshAgent = true;

        UnityEngine.AI.NavMeshAgent navMeshAgent;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.applyRootMotion = true;

            if (navMeshAgent == null)
            {
                animator.TryGetComponent(out navMeshAgent);
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = animator.GetComponentInParent<NavMeshAgent>();
            }
                
            if (navMeshAgent != null && disableNavMeshAgent)
            {
                navMeshAgent.enabled = false;
            }
        }
    }
}
