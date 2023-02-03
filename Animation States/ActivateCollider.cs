using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ActivateCollider : StateMachineBehaviour
    {
        Collider collider;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var deathCollider = animator.gameObject.GetComponentInChildren<DeathColliderRef>();

            if (deathCollider == null)
            {
                return;
            }

            BoxCollider collider = deathCollider.GetComponent<BoxCollider>();

            collider.enabled = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
}
