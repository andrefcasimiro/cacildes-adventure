
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

        public override void OnStateUpdate(Animator enemy, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.enemy.enemyHealthController.currentHealth > 0)
            {
                this.enemy.characterController.Move(-1f * 2f * Time.deltaTime * enemy.transform.forward);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.StopFall();
        }


    }

}
