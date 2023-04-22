using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class EnemyState_Falling : StateMachineBehaviour
    {
        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.rigidbody.useGravity = true;
            enemy.rigidbody.isKinematic = false;
            enemy.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Transform objectEnemyIsOn = enemy.IsGrounded();
            if (
                objectEnemyIsOn != null
                && !objectEnemyIsOn.CompareTag("Water")
            )
            {
                enemy.ReenableNavmesh();
            }

            // To prevent enemy from being stuck
            enemy.transform.position += enemy.transform.forward * -1f * 5f * Time.deltaTime;
        }


    }

}
