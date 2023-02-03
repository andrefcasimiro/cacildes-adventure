using UnityEngine;

namespace AF
{
    public class EnemyState_Dying : StateMachineBehaviour
    {
        EnemyManager enemy;

        public bool turnKinematic = true;
        public bool disableCapsuleCollider = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

            var deathCollider = animator.gameObject.GetComponentInChildren<DeathColliderRef>();
            if (deathCollider != null)
            {
                deathCollider.GetComponent<BoxCollider>().enabled = true;
            }

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.GetComponent<Rigidbody>().isKinematic = turnKinematic;
            enemy.GetComponent<CapsuleCollider>().enabled = !disableCapsuleCollider;

            enemy.enemyHealthController.DisableHealthHitboxes();

            enemy.enemyHealthController.onEnemyDeath.Invoke();

            enemy.agent.enabled = false;
            enemy.enabled = false;
        }
    }
}
