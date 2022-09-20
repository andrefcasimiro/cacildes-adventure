using UnityEngine;

namespace AF
{

    public class DisableOnDeath : StateMachineBehaviour
    {
        Enemy enemy;

        public bool turnKinematic = true;
        public bool disableCapsuleCollider = true;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            enemy.GetComponent<Rigidbody>().isKinematic = turnKinematic;
            enemy.GetComponent<CapsuleCollider>().enabled = !disableCapsuleCollider;


            var healthbox = enemy.GetComponent<EnemyHealthbox>();
            if (healthbox != null && healthbox.healthBarSlider != null)
            {
                healthbox.healthBarSlider.gameObject.SetActive(false);
            }

            if (enemy.onEnemyDeath != null)
            {
                enemy.onEnemyDeath.Invoke();
            }
            enemy.GetComponent<Healthbox>().enabled = false;
            enemy.agent.enabled = false;
            enemy.enabled = false;
        }

    }
}
