
using UnityEngine;
using UnityEngine.AI;

namespace AF {
    public class EnemyState_AvoidInvalidPaths : StateMachineBehaviour
    {
        NavMeshAgent navMeshAgent;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (navMeshAgent == null)
            {
                animator.TryGetComponent(out navMeshAgent);
            }

            if (navMeshAgent == null)
            {
                navMeshAgent = animator.GetComponentInParent<NavMeshAgent>();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(animator.transform.position, animator.transform.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {

                NavMesh.SamplePosition(animator.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas);

                navMeshAgent.nextPosition = hit.position != null ? hit.position : animator.transform.position;
                navMeshAgent.updatePosition = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            navMeshAgent.updatePosition = false;
        }
    }
}
