using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class EnemyStateWaiting : StateMachineBehaviour
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
            enemy.EnableHealthHitboxes();

            enemy.DisableAllWeaponHitboxes();

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
            if (enemy.canCircleAround)
            {
                float dice = Random.Range(0, 100);

                if (dice <= enemy.circleAroundWeight)
                {
                    float randomDir = Random.Range(0, 1);
                    if (randomDir < 0.5)
                    {
                        animator.CrossFade(enemy.circleAroundRightAnimation, 0.05f);
                    }
                    else
                    {
                        animator.CrossFade(enemy.circleAroundLeftAnimation, 0.05f);
                    }

                    return;
                }
            }


            float turnChance = Random.Range(enemy.minWaitingTimeBeforeResumingCombat, enemy.maxWaitingTimeBeforeResumingCombat);
            enemy.turnWaitingTime = turnChance;

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.CheckForDodgeChance();
        }

        


    }

}
