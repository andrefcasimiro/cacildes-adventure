using AF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyActivateHitboxOnStateEnter : StateMachineBehaviour
    {
        EnemyManager enemy;

        public bool activateLeftHand = false;
        public bool activateRightHand = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (activateLeftHand)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }

            if (activateRightHand)
            {
                enemy.enemyWeaponController.ActivateRightHandHitbox();
            }
        }
    }

}
