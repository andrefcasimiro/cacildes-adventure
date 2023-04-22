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

            enemy.rigidbody.useGravity = false;
            enemy.rigidbody.isKinematic = true;


            // Activate health hitboxes after dodging for safety
            enemy.enemyHealthController.EnableHealthHitboxes();

            enemy.enemyWeaponController.DisableAllWeaponHitboxes();

            if (enemy.canFall)
            {
                if (enemy.IsGrounded() != null)
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
                        animator.Play(enemy.enemyCombatController.circleAroundRightAnimation);
                    }
                    else
                    {
                        animator.Play(enemy.enemyCombatController.circleAroundLeftAnimation);
                    }

                    return;
                }
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
            else // We've waited long enough, request new combat decision
            {
                animator.SetBool(enemy.hashIsWaiting, false);
            }
        }
    }
}
