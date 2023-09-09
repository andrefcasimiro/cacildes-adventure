
using UnityEngine;

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

            enemy.BeginFall();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.enemyHealthController.currentHealth > 0)
            {
                enemy.characterController.Move(animator.transform.forward * -1f * 2f * Time.deltaTime);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.StopFall();
        }


    }

}
