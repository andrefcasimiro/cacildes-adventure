using UnityEngine;

namespace AF
{

    public class DisableOnDeath : StateMachineBehaviour
    {
        Enemy enemy;
        EnemyHealthController enemyHealthController;

        public bool turnKinematic = true;
        public bool disableCapsuleCollider = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyHealthController == null)
            {
                enemyHealthController = enemy.GetComponent<EnemyHealthController>();
            }

            enemy.GetComponent<Rigidbody>().isKinematic = turnKinematic;
            enemy.GetComponent<CapsuleCollider>().enabled = !disableCapsuleCollider;

            if (enemyHealthController != null)
            {
                enemyHealthController.DisableHealthHitboxes();
            }

            if (enemyHealthController.onEnemyDeath != null)
            {
                enemyHealthController.onEnemyDeath.Invoke();
            }
            enemy.agent.enabled = false;
            enemy.enabled = false;
        }

    }
}
