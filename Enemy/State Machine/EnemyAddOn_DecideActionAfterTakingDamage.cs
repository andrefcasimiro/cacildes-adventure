using UnityEngine;

namespace AF
{

    public class EnemyAddOn_DecideActionAfterTakingDamage : StateMachineBehaviour
    {
        EnemyManager enemy;

        [Header("Options")]
        [Range(0, 100)] public int chanceToTeleport = 50;
        [Range(0, 100)] public int chanceToUseBuff = 25;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.gameObject.TryGetComponent(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var chance = Random.Range(0, 100);

            if (chance > chanceToTeleport && enemy.enemyTeleportManager != null)
            {
                enemy.enemyTeleportManager.BeginTeleport();
            }
            else if (chance > chanceToUseBuff)
            {
                enemy.enemyBuffController.PickRandomBuff();
            }
        }

    }

}
