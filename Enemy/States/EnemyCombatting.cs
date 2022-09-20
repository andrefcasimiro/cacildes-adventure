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

        [Header("Animation Names")]
        public string[] meleeLightAttacks;
        public string[] meleeHeavyAttacks;
        public string[] meleeRangeAttacks;
        public string[] dodgeActions;
        public string[] blockActions;
        public string combatIdle = "Waiting";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            enemy.TryGetComponent(out enemyCombatable);

            // Early exit to Chase state
            if (enemy.player.IsBusy() || IsPlayerFarAway())
            {
                animator.SetBool(enemy.hashCombatting, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            if (enemy.player.isAttacking)
            {
                RespondToPlayer(animator);
            }
            else
            {
                AttackPlayer(animator);
            }

        }


        public void AttackPlayer(Animator animator)
        {
            bool playerIsClose = Vector3.Distance(enemy.player.transform.position, enemy.transform.position) <= 1f;
            if (playerIsClose)
            {
                float chanceToBlockOrDodge = Random.Range(0, 1f);

                if (chanceToBlockOrDodge <= 0.1f)
                {
                    if (this.blockActions.Length > 0)
                    {
                        animator.Play(this.blockActions[0]);
                        return;
                    }
                    else if (this.dodgeActions.Length > 0)
                    {
                        animator.Play(this.dodgeActions[0]);
                        return;
                    }
                }

            }


            float attackDice = Random.Range(0, 1f);

            if (attackDice <= 0.25f)
            {
                animator.Play(combatIdle);
                return;
            }

            int attackClip = 0;

            if (attackDice >= enemy.heavyAttackFrequency && this.meleeHeavyAttacks.Length > 0)
            {
                attackClip = Random.Range(0, this.meleeHeavyAttacks.Length);
                animator.Play(this.meleeHeavyAttacks[attackClip]);
                return;
            }

            attackClip = Random.Range(0, this.meleeLightAttacks.Length);
            animator.Play(this.meleeLightAttacks[attackClip]);
        }

        public void RespondToPlayer(Animator animator)
        {
            if (!enemy.player.isAttacking)
            {
                animator.Play(combatIdle);
                return;
            }

            float blockDice = Random.Range(0, 1f);
            if (blockDice >= enemy.blockFrequency &&  this.blockActions.Length > 0)
            {
                // Show Shield
                if (enemyCombatable != null && enemyCombatable.GetShieldInstance() != null)
                {
                    enemyCombatable.GetShieldInstance().gameObject.SetActive(true);
                }

                int blockClip = Random.Range(0, this.blockActions.Length);
                animator.Play(this.blockActions[blockClip]);
                return;
            }

            float dodgeDice = Random.Range(0, 1f);
            Debug.Log("dodgeDice" + dodgeDice);
            if (dodgeDice >= enemy.dodgeFrequency && this.dodgeActions.Length > 0)
            {
                enemy.GetComponent<EnemyHealthbox>().ActivateDodge();

                int dodgeClip = Random.Range(0, this.dodgeActions.Length);
                animator.Play(this.dodgeActions[dodgeClip]);
                return;
            }

            animator.Play(combatIdle);
        }

        public bool IsPlayerFarAway()
        {
            return Vector3.Distance(enemy.agent.transform.position, enemy.player.transform.position) > enemy.agent.stoppingDistance + 0.5f;
        }
    }

}
