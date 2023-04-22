using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AF
{
    public class EnemyState_Combatting : StateMachineBehaviour
    {
        EnemyManager enemy;
        Animator animator;

        [Header("Legacy Animation Names")]
        [Tooltip("Use this for most animators, skip for the generic humanoid which will get its animations from the EnemyCombatController")]
        public string[] meleeLightAttacks;
        [Tooltip("Use this for most animators, skip for the generic humanoid which will get its animations from the EnemyCombatController")]
        public string[] meleeHeavyAttacks;

        public string[] meleeRangeAttacks;
        public string[] blockActions;
        public string[] shootActions;

        Dictionary<EnemyCombatController.CombatAction, System.Func<bool>> attackPlayerFlow = new Dictionary<EnemyCombatController.CombatAction, System.Func<bool>>();
        Dictionary<EnemyCombatController.CombatAction, System.Func<bool>> respondToPlayerFlow = new Dictionary<EnemyCombatController.CombatAction, System.Func<bool>>();

        PlayerCombatController playerCombatController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            this.animator = animator;

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (playerCombatController == null)
            {
                playerCombatController = enemy.player.GetComponent<PlayerCombatController>();
            }

            if (enemy.enemyBehaviorController.isAgressive == false)
            {
                enemy.enemyBehaviorController.TurnAgressive();
            }

            if (enemy.enemyHealthController != null)
            {
                enemy.enemyHealthController.ShowHUD();
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

            enemy.enemyWeaponController.DisableAllWeaponHitboxes();
    
            if (attackPlayerFlow.Count <= 0)
            {
                foreach (var combatFlow in enemy.enemyCombatController.attackPlayer)
                {
                    SetupCombatFlow(combatFlow, attackPlayerFlow);
                }
            }

            if (respondToPlayerFlow.Count <= 0)
            {
                foreach (var combatFlow in enemy.enemyCombatController.respondToPlayer)
                {
                    SetupCombatFlow(combatFlow, respondToPlayerFlow);
                }
            }

            if (enemy.enemyCombatController.IsPlayerFarAway() && enemy.canChase)
            {
                animator.SetBool(enemy.hashCombatting, false);
                animator.SetBool(enemy.hashChasing, true);
                return;
            }

            // Then evaluate combat decision
            if (playerCombatController.isCombatting)
            {
                RespondToPlayer();
            }
            else
            {
                AttackPlayer();
            }
        }

        void SetupCombatFlow(EnemyCombatController.CombatAction action, Dictionary<EnemyCombatController.CombatAction, System.Func<bool>> flow)
        {
            if (action == EnemyCombatController.CombatAction.Shoot)
            {
                flow.Add(action, this.ShootDice);
            }
            else if (action == EnemyCombatController.CombatAction.LightAttack)
            {
                flow.Add(action, this.LightAttackDice);
            }
            else if (action == EnemyCombatController.CombatAction.HeavyAttack)
            {
                flow.Add(action, this.HeavyAttackDice);
            }
            else if (action == EnemyCombatController.CombatAction.Block)
            {
                flow.Add(action, this.BlockDice);
            }
            else if (action == EnemyCombatController.CombatAction.UseBuff)
            {
                flow.Add(action, this.UseBuffDice);
            }
        }

        public void RespondToPlayer()
        {
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
            foreach (var combatAction in attackPlayerFlow)
            {
                var combatActionValue = combatAction.Value();

                if (combatActionValue)
                {
                    break;
                }
            }
        }

        bool ShootDice()
        {
            if (enemy.enemyProjectileController == null)
            {
                return false;
            }

            int dice = Random.Range(0, 100);
            if (dice < enemy.enemyProjectileController.shootingWeight && enemy.enemyProjectileController.CanShoot() && this.shootActions.Length > 0)
            {
                int clip = Random.Range(0, this.shootActions.Length);
                animator.Play(this.shootActions[clip]);

                return true;
            }

            return false;
        }

        bool BlockDice()
        {
            if (enemy.enemyBlockController == null)
            {
                return false;
            }

            int blockDice = Random.Range(0, 100);
            if (blockDice < enemy.enemyBlockController.blockWeight && this.blockActions.Length > 0)
            {
                int blockClip = Random.Range(0, this.blockActions.Length);
                animator.Play(this.blockActions[blockClip]);
                return true;
            }
            return false;
        }

        bool LightAttackDice()
        {
            var attacks = enemy.enemyCombatController.meleeLightAttacks.Length > 0 ? enemy.enemyCombatController.meleeLightAttacks : meleeLightAttacks;
            int attackDice = Random.Range(0, 100);

            if (attackDice < enemy.enemyCombatController.lightAttackWeight && attacks.Length > 0)
            {
                int attackClip = Random.Range(0, attacks.Length);
                animator.Play(attacks[attackClip]);
                return true;
            }

            return false;
        }

        bool HeavyAttackDice()
        {
            var attacks = enemy.enemyCombatController.meleeHeavyAttacks.Length > 0 ? enemy.enemyCombatController.meleeHeavyAttacks : meleeHeavyAttacks;
            int attackDice = Random.Range(0, 100);

            if (attackDice < enemy.enemyCombatController.lightAttackWeight && attacks.Length > 0)
            {
                int attackClip = Random.Range(0, attacks.Length);
                animator.Play(attacks[attackClip]);
                return true;
            }

            return false;
        }

        bool UseBuffDice()
        {
            int dice = Random.Range(0, 100);

            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return false;
            }

            // Shuffle buffs
            var shuffledBuffs = Randomize(enemy.enemyBuffController.buffs.ToArray());

            var buffChosen = shuffledBuffs.FirstOrDefault(x => dice < x.weight && enemy.enemyBuffController.CanUseBuff(x));

            if (buffChosen != null)
            {
                enemy.enemyBuffController.PrepareBuff(buffChosen);
                return true;
            }

            return false;
        }

        IEnumerable<EnemyBuffController.Buff> Randomize(EnemyBuffController.Buff[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }

}
