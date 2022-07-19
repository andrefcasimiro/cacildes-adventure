using UnityEngine;

namespace AF
{

    /// <summary>
    /// Combat works by a mix of utilising StateEnter to fire the decision of the combat turn
    /// and then always making sure we have SetTrigger on the animator to escape the Combat state
    /// </summary>
    public class EnemyCombatting : StateMachineBehaviour
    {
        Enemy enemy;

        ICombatable enemyCombatable;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            enemy.TryGetComponent(out enemyCombatable);
            if (enemyCombatable != null && enemyCombatable.GetShieldInstance() != null)
            {
                enemyCombatable.GetShieldInstance().gameObject.SetActive(false);
            }

            // Early exit to Chase state
            if (enemy.player.IsNotAvailable() || IsPlayerFarAway())
            {
                animator.SetBool(enemy.hashCombatting, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            Utils.FaceTarget(enemy.transform, enemy.player.transform);

            if (enemy.player.isAttacking)
            {
                RespondToPlayer(animator);
            }
            else
            {
                float chanceToAttack = Random.Range(0, 1f);

                AttackPlayer(animator);

            }
        }


        public void AttackPlayer(Animator animator)
        {
            float attackDice = Random.Range(0, 1f);

            if (attackDice > 0 && attackDice <= 0.25f)
            {
                animator.SetTrigger(enemy.hashAttacking1);
            }
            else if (attackDice > 0.25f && attackDice <= 0.5f)
            {
                animator.SetTrigger(enemy.hashAttacking2);
            }
            else if (attackDice > 0.5f && attackDice <= 0.85f)
            {
                animator.SetTrigger(enemy.hashAttacking3);
            }
            else
            {
                animator.SetTrigger(enemy.hashWaiting);
            }
        }

        public void RespondToPlayer(Animator animator)
        {
            if (!enemy.player.isAttacking)
            {
                animator.SetTrigger(enemy.hashWaiting);
                return;
            }

            float blockDice = Random.Range(enemy.minBlockChance, enemy.maxBlockChance);
            float dodgeDice = Random.Range(enemy.minDodgeChange, enemy.maxDodgeChance);

            if (blockDice >= enemy.blockChance)
            {
                // Show Shield
                if (enemyCombatable != null && enemyCombatable.GetShieldInstance() != null)
                {
                    enemyCombatable.GetShieldInstance().gameObject.SetActive(true);
                }

                Utils.FaceTarget(enemy.transform, enemy.player.transform);
                animator.SetTrigger(enemy.hashBlocking);
            }
            else if (dodgeDice >= enemy.dodgeChance)
            {
                Utils.FaceTarget(enemy.transform, enemy.player.transform);

                float dodgeClipDice = Random.Range(0f, 1f);
                if (dodgeClipDice > 0.5f)
                {
                    animator.SetTrigger(enemy.hashDodging);
                }
                else
                {
                    animator.SetTrigger(enemy.hashWaiting);
                }
            }
            else
            {
                animator.SetTrigger(enemy.hashWaiting);
            }
        }

        public bool IsPlayerFarAway()
        {
            return Vector3.Distance(enemy.agent.transform.position, enemy.player.transform.position) > enemy.agent.stoppingDistance + 0.5f;
        }

    }

}
