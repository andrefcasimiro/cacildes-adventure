using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using System.Linq;

namespace AF
{

    /// <summary>
    /// Combat works by a mix of utilising StateEnter to fire the decision of the combat turn
    /// and then always making sure we have SetTrigger on the animator to escape the Combat state
    /// </summary>
    public class EnemyStateCombatting : StateMachineBehaviour
    {
        EnemyManager enemy;
        Animator animator;

        [Header("Animation Names")]
        public string[] meleeLightAttacks;
        public string[] meleeHeavyAttacks;
        public string[] meleeRangeAttacks;
        public string[] blockActions;
        public string[] shootActions;

        Dictionary<CombatAction, System.Func<bool>> attackPlayerFlow = new Dictionary<CombatAction, System.Func<bool>>();
        Dictionary<CombatAction, System.Func<bool>> respondToPlayerFlow = new Dictionary<CombatAction, System.Func<bool>>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            this.animator = animator;

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            // Notify active player companions
            CompanionManager[] companionManagers = FindObjectsOfType<CompanionManager>();
            foreach (var companion in companionManagers)
            {
                if (companion.IsInCombat() == false)
                {
                    companion.ForceIntoCombat(enemy);
                }
            }

            enemy.DisableAllWeaponHitboxes();
    
            if (attackPlayerFlow.Count <= 0)
            {
                foreach (var combatFlow in enemy.attackPlayer)
                {
                    SetupCombatFlow(combatFlow, attackPlayerFlow);
                }
            }

            if (respondToPlayerFlow.Count <= 0)
            {
                foreach (var combatFlow in enemy.attackPlayer)
                {
                    SetupCombatFlow(combatFlow, respondToPlayerFlow);
                }
            }

            if (IsPlayerFarAway())
            {
                animator.SetBool(enemy.hashCombatting, false);
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
            else if (action == CombatAction.UseBuff)
            {
                flow.Add(action, this.UseBuffDice);
            }
        }

        public void RespondToPlayer()
        {
            if (!enemy.playerCombatController.isCombatting)
            {
                animator.SetBool(enemy.hashIsWaiting, true);
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
            if (attackPassivityChance <= enemy.passivityWeight)
            {
                animator.SetBool(enemy.hashIsWaiting, true);
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
            if (dice < enemy.shootingWeight && enemy.CanShoot() && this.shootActions.Length > 0)
            {
                int clip = Random.Range(0, this.shootActions.Length);
                animator.Play(this.shootActions[clip]);

                return true;
            }

            return false;
        }

        bool BlockDice()
        {
            int blockDice = Random.Range(0, 100);
            if (blockDice < enemy.blockWeight && this.blockActions.Length > 0)
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
            if (attackDice < enemy.lightAttackWeight && this.meleeLightAttacks.Length > 0)
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
            if (attackDice < enemy.heavyAttackWeight && this.meleeHeavyAttacks.Length > 0)
            {
                attackClip = Random.Range(0, this.meleeHeavyAttacks.Length);
                animator.Play(this.meleeHeavyAttacks[attackClip]);
                return true;
            }

            return false;
        }

        bool UseBuffDice()
        {
            int dice = Random.Range(0, 100);

            int clip = 0;

            if (enemy.buffs.Length <= 0)
            {
                return false;
            }

            // Shuffle buffs
            var shuffledBuffs = Randomize(enemy.buffs.ToArray());

            var buffChosen = shuffledBuffs.FirstOrDefault(x => dice < x.weight && enemy.CanUseBuff(x));

            if (buffChosen != null)
            {
                enemy.PrepareBuff(buffChosen);
                return true;
            }

            return false;
        }
        #endregion

        IEnumerable<Buff> Randomize(Buff[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }

        public bool IsPlayerFarAway()
        {
            if (enemy.ignoreCompanions == false && enemy.currentCompanion != null)
            {
                return false;
            }

            return Vector3.Distance(enemy.agent.transform.position, enemy.playerCombatController.transform.position) > enemy.agent.stoppingDistance + 0.5f;
        }
    }

}
