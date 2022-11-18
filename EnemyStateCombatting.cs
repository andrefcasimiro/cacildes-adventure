using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

namespace AF
{

    /// <summary>
    /// Combat works by a mix of utilising StateEnter to fire the decision of the combat turn
    /// and then always making sure we have SetTrigger on the animator to escape the Combat state
    /// </summary>
    public class EnemyStateCombatting : StateMachineBehaviour
    {
        Enemy enemy;
        EnemyCombatController enemyCombatController;
        EnemyBlockController enemyBlockController;
        EnemyDodgeController enemyDodgeController;
        EnemyHealthController enemyHealthController;
        EnemyProjectileController enemyProjectileController;
        Animator animator;

        [Header("Animation Names")]
        public string[] meleeLightAttacks;
        public string[] meleeHeavyAttacks;
        public string[] meleeRangeAttacks;
        public string[] dodgeActions;
        public string[] blockActions;
        public string[] shootActions;

        [Header("Dodge Options")]
        public bool dodgeLeftOrRight = false;

        Dictionary<CombatAction, System.Func<bool>> attackPlayerFlow = new Dictionary<CombatAction, System.Func<bool>>();
        Dictionary<CombatAction, System.Func<bool>> respondToPlayerFlow = new Dictionary<CombatAction, System.Func<bool>>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            this.animator = animator;

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyCombatController == null)
            {
                enemyCombatController = enemy.GetComponent<EnemyCombatController>();
            }
            enemyCombatController.DisableAllWeaponHitboxes();
    
            if (enemyBlockController == null)
            {
                enemyBlockController = enemy.GetComponent<EnemyBlockController>();
            }

            if (enemyDodgeController == null)
            {
                enemyDodgeController = enemy.GetComponent<EnemyDodgeController>();
            }

            if (enemyProjectileController == null)
            {
                enemyProjectileController = enemy.GetComponent<EnemyProjectileController>();
            }

            if (attackPlayerFlow.Count <= 0)
            {
                foreach (var combatFlow in enemyCombatController.attackPlayer)
                {
                    SetupCombatFlow(combatFlow, attackPlayerFlow);
                }
            }
            if (respondToPlayerFlow.Count <= 0)
            {
                foreach (var combatFlow in enemyCombatController.attackPlayer)
                {
                    SetupCombatFlow(combatFlow, respondToPlayerFlow);
                }
            }

            if (IsPlayerFarAway())
            {
                animator.SetBool(enemyCombatController.hashCombatting, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            if (enemy.playerCombatController.isCombatting)
            {
                RespondToPlayer();
            }
            else
            {
                AttackPlayer();
            }
        }

        void SetupCombatFlow(CombatAction action, Dictionary<CombatAction, System.Func<bool>> flow)
        {
            if (action == CombatAction.Shoot)
            {
                flow.Add(action, this.ShootDice);
            }
            else if (action == CombatAction.LightAttack)
            {
                flow.Add(action, this.LightAttackDice);
            }
            else if (action == CombatAction.HeavyAttack)
            {
                flow.Add(action, this.HeavyAttackDice);
            }
            else if (action == CombatAction.Block)
            {
                flow.Add(action, this.BlockDice);
            }
            else if (action == CombatAction.Dodge)
            {
                flow.Add(action, this.DodgeDice);
            }
        }

        public void RespondToPlayer()
        {
            if (!enemy.playerCombatController.isCombatting)
            {
                animator.SetBool(enemyCombatController.hashIsWaiting, true);
                return;
            }

            foreach (var combatAction in attackPlayerFlow)
            {
                var combatActionValue = combatAction.Value();
                if (combatActionValue)
                {
                    break;
                }
            }

        }

        public void AttackPlayer()
        {
            int attackPassivityChance = Random.Range(0, 100);
            if (attackPassivityChance <= enemyCombatController.passivityWeight)
            {
                animator.SetBool(enemyCombatController.hashIsWaiting, true);
                return;
            }

            foreach (var combatAction in attackPlayerFlow)
            {
                var combatActionValue = combatAction.Value();

                if (combatActionValue)
                {
                    break;
                }
            }
        }

        #region Dices
        bool ShootDice()
        {
            int dice = Random.Range(0, 100);
            if (dice < enemyProjectileController.weight && enemyProjectileController.CanShoot() && this.shootActions.Length > 0)
            {
                int clip = Random.Range(0, this.shootActions.Length);
                animator.Play(this.shootActions[clip]);

                return true;
            }

            return false;
        }

        bool DodgeDice()
        {
            int dodgeDice = Random.Range(0, 100);
            if (dodgeDice < enemyDodgeController.weight && this.dodgeActions.Length > 0)
            {
                if (dodgeLeftOrRight)
                {
                    NavMeshPath navMeshPath = new NavMeshPath();
                    //create path and check if it can be done
                    // and check if navMeshAgent can reach its target
                    if (enemy.agent.CalculatePath(enemy.transform.right, navMeshPath) && navMeshPath.status != NavMeshPathStatus.PathInvalid)
                    {
                        animator.Play("Dodge_Right");
                    }
                    else if (enemy.agent.CalculatePath(enemy.transform.right * -1, navMeshPath) && navMeshPath.status != NavMeshPathStatus.PathInvalid)
                    {
                        animator.Play("Dodge_Left");
                    }

                    return true;
                }

                int dodgeClip = Random.Range(0, this.dodgeActions.Length);
                animator.Play(this.dodgeActions[dodgeClip]);

                return true;
            }

            return false;
        }

        bool BlockDice()
        {
            int blockDice = Random.Range(0, 100);
            if (blockDice < enemyBlockController.weight && this.blockActions.Length > 0)
            {
                int blockClip = Random.Range(0, this.blockActions.Length);
                animator.Play(this.blockActions[blockClip]);
                return true;
            }
            return false;
        }

        bool LightAttackDice()
        {
            int attackDice = Random.Range(0, 100);
            int attackClip = 0;
            if (attackDice < enemyCombatController.lightAttackWeight && this.meleeLightAttacks.Length > 0)
            {
                attackClip = Random.Range(0, this.meleeLightAttacks.Length);
                animator.Play(this.meleeLightAttacks[attackClip]);
                return true;
            }

            return false;
        }

        bool HeavyAttackDice()
        {
            int attackDice = Random.Range(0, 100);
            int attackClip = 0;
            if (attackDice < enemyCombatController.heavyAttackWeight && this.meleeHeavyAttacks.Length > 0)
            {
                attackClip = Random.Range(0, this.meleeHeavyAttacks.Length);
                animator.Play(this.meleeHeavyAttacks[attackClip]);
                return true;
            }

            return false;
        }
        #endregion

        public bool IsPlayerFarAway()
        {
            return Vector3.Distance(enemy.agent.transform.position, enemy.playerCombatController.transform.position) > enemy.agent.stoppingDistance + 0.5f;
        }
    }

}
