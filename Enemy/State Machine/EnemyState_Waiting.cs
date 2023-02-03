using UnityEngine;

namespace AF
{

    public class EnemyState_Waiting : StateMachineBehaviour
    {        
        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }
            
            // Activate health hitboxes after dodging for safety
            enemy.enemyHealthController.EnableHealthHitboxes();

            enemy.enemyWeaponController.DisableAllWeaponHitboxes();

            if (enemy.canFall)
            {
                if (enemy.IsGrounded())
                {
                    if (enemy.agent.updatePosition == false)
                    {
                        enemy.ReenableNavmesh();
                    }
                }
                else
                {
                    animator.Play(enemy.hashFalling);
                }
            }

            // On State enter, evaluate if we should circle around
            if (enemy.enemyCombatController.circleAroundWeight > 0)
            {
                float dice = Random.Range(0, 100);

                if (dice <= enemy.enemyCombatController.circleAroundWeight)
                {
                    float randomDir = Random.Range(0, 1);
                    if (randomDir < 0.5)
                    {
                        animator.CrossFade(enemy.enemyCombatController.circleAroundRightAnimation, 0.1f);
                    }
                    else
                    {
                        animator.CrossFade(enemy.enemyCombatController.circleAroundLeftAnimation, 0.1f);
                    }

                    return;
                }
            }

            float turnChance = Random.Range(enemy.enemyCombatController.minimumDecisionTime, enemy.enemyCombatController.maximumDecisionTime);
            enemy.enemyCombatController.turnDecisionTime = turnChance;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.enemyDodgeController.CheckForDodgeChance();
        }
    }
}
