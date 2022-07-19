using UnityEngine;

namespace AF
{

    public class DisableOnDeath : StateMachineBehaviour
    {
        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            enemy.GetComponent<Rigidbody>().isKinematic = true;
            enemy.GetComponent<CapsuleCollider>().enabled = false;
            enemy.GetComponent<EnemyHealthbox>().healthBarSlider.gameObject.SetActive(false);
            enemy.GetComponent<Healthbox>().enabled = false;
            enemy.agent.enabled = false;
            enemy.enabled = false;
        }

    }
}
