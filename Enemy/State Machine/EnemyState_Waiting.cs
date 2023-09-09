using UnityEngine;

namespace AF
{

    public class EnemyState_Waiting : StateMachineBehaviour
    {
        public float minimumDecisionTime = 0.25f;
        public float maximumDecisionTime = 2f;
        float turnDecisionTime;
        float waitingCounter = 0;

        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (enemy.enemyCombatController.hasHyperArmorActive)
            {
                enemy.enemyCombatController.hasHyperArmorActive = false;
            }

            // Activate health hitboxes after dodging for safety
            enemy.enemyHealthController.EnableHealthHitboxes();

            enemy.enemyWeaponController.DisableAllWeaponHitboxes();

            if (enemy.enemyCombatController.minimumDecisionTimeOverride != -1)
            {
                minimumDecisionTime = enemy.enemyCombatController.minimumDecisionTimeOverride;
            }
            if (enemy.enemyCombatController.maximumDecisionTimeOverride != -1)
            {
                minimumDecisionTime = enemy.enemyCombatController.maximumDecisionTimeOverride;
            }

            turnDecisionTime = Random.Range(minimumDecisionTime, maximumDecisionTime);
            waitingCounter = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.enemyDodgeController != null)
            {
                enemy.enemyDodgeController.CheckForDodgeChance();
            }

            if (waitingCounter < turnDecisionTime)
            {
                waitingCounter += Time.deltaTime;
            }
            else
            // We've waited long enough, request new combat decision
            {
                // Decide if we are going to strafe enxt
                if (enemy.enemyCombatController.circleAroundWeight > 0 && Random.Range(0, 100) < enemy.enemyCombatController.circleAroundWeight)
                {
                    float randomDir = Random.Range(0, 1);
                    if (randomDir < 0.5)
                    {
                        animator.Play(enemy.enemyCombatController.circleAroundRightAnimation);
                    }
                    else
                    {
                        animator.Play(enemy.enemyCombatController.circleAroundLeftAnimation);
                    }
                }
                else
                {
                    animator.SetBool(enemy.hashIsWaiting, false);
                }
            }
        }
    }
}
