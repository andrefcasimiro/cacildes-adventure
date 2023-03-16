﻿using UnityEngine;

namespace AF
{
    public class EnemyState_Dying : StateMachineBehaviour
    {
        EnemyManager enemy;

        public bool turnKinematic = true;
        public bool disableCapsuleCollider = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.GetComponent<Rigidbody>().isKinematic = turnKinematic;

            if (disableCapsuleCollider)
            {
                var capsuleColliders = enemy.GetComponents<CapsuleCollider>();
                foreach (var capsuleCollider in capsuleColliders)
                {
                    capsuleCollider.enabled = false;
                }
            }

            var deathCollider = animator.gameObject.GetComponentInChildren<DeathColliderRef>();
            if (deathCollider != null)
            {
                deathCollider.GetComponent<BoxCollider>().enabled = true;
            }

            enemy.enemyHealthController.DisableHealthHitboxes();
            enemy.enemyHealthController.onEnemyDeath.Invoke();

            enemy.agent.enabled = false;
            enemy.enabled = false;
        }
    }
}
