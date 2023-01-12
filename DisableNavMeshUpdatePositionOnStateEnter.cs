using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class DisableNavMeshUpdatePositionOnStateEnter : StateMachineBehaviour
    {
        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.TryGetComponent<EnemyManager>( out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>();
            }

            enemy.rigidbody.useGravity = true;
            enemy.rigidbody.isKinematic = false;
            enemy.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            
            enemy.agent.updatePosition = false;
        }
    }

}