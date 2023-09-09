using UnityEngine;

namespace AF
{

    public class EnemyAddOn_DecideActionAfterTakingDamage : StateMachineBehaviour
    {
        EnemyManager enemy;

        EnemyTeleportManager enemyTeleportManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemyTeleportManager = enemy.enemyTeleportManager;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemyTeleportManager == null)
            {
                return;
            }

            var chance = Random.Range(0, 100);

            if (chance > enemyTeleportManager.chanceToTeleport)
            {
                enemy.enemyTeleportManager.BeginTeleport();
            }
            else if (chance > enemyTeleportManager.chanceToUseBuff)
            {
                enemy.enemyBuffController.PickRandomBuff();
            }
        }

    }

}
