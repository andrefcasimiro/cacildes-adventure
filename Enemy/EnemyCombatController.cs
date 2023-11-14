using System.Linq;
using UnityEngine;

namespace AF
{
    public class EnemyCombatController : MonoBehaviour
    {
        public enum CombatAction
        {
            Shoot,
            LightAttack,
            HeavyAttack,
            UseRunningAttack,
            Block,
            UseBuff,
        }

        [System.Serializable]
        public class RunningAttack
        {
            public float minDistanceToAttack;
            public string runningAttack;
        }

        [Header("Combat Flow - Action Sequences")]
        public CombatAction[] respondToPlayer;
        public CombatAction[] attackPlayer;

        [Header("Light Attacks")]
        [Range(0, 100)] public int lightAttackWeight = 75;

        [Header("Heavy Attacks")]
        [Range(0, 100)] public int heavyAttackWeight = 25;

        [Header("Circle Around Settings")]
        [Range(0, 100)] public float circleAroundWeight = 75;
        public string circleAroundRightAnimation = "Strafe Right";
        public string circleAroundLeftAnimation = "Strafe Left";

        [Header("Passivity")]
        public float minimumDecisionTimeOverride = -1f;
        public float maximumDecisionTimeOverride = -1f;

        [Header("Override Animator Combat Attacks")]
        public string[] meleeLightAttacks;
        public string[] meleeHeavyAttacks;

        [Header("Light Attacks Below 50%")]
        public string[] meleeLightAttacksForSecondPhase;

        [Header("Distance To Player")]
        public float minimumAttackDistanceToPlayer = 0;

        [Header("Running Attack")]
        public RunningAttack[] runningAttacks;

        // Stats affected by animations
        [HideInInspector] public int weaponDamage = 100;
        [HideInInspector] public int weaponDamageBonus = 0;
        [HideInInspector] public StatusEffect weaponStatusEffect = null;
        [HideInInspector] public float statusEffectAmount = 0f;
        [HideInInspector] public float bonusBlockStaminaCost = 0f;
        [HideInInspector] public int currentAttackPoiseDamage = 1;
        [HideInInspector] public bool hasHyperArmorActive = false;
        [HideInInspector] public string transitionToExecution = "";
        [HideInInspector] public string playerExecutedClip = "";

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        public bool isDamagingHimself = false;

        public void ForceIntoCombat()
        {
            if (enemyManager.enemyHealthController.currentHealth <= 0)
            {
                return;
            }

            enemyManager.animator.SetBool(enemyManager.hashChasing, true);
        }

        public int GetCurrentAttack()
        {
            var attackPower = Formulas.CalculateAIAttack(enemyManager.enemy.basePhysicalAttack, enemyManager.currentLevel) + weaponDamageBonus;

            if (enemyManager.attackReducingFactor > 1)
            {
                attackPower = (int)(attackPower / enemyManager.attackReducingFactor);
            }

            return attackPower;
        }

        public bool IsInCombat()
        {
            if (!this.isActiveAndEnabled)
            {
                return false;
            }

            return enemyManager.animator.GetBool(enemyManager.hashIsInCombat);
        }

        public bool IsWaiting()
        {
            return enemyManager.animator.GetBool(enemyManager.hashIsWaiting);
        }

        public bool IsPlayerFarAway()
        {
            return Vector3.Distance(enemyManager.agent.transform.position, enemyManager.player.transform.position) > enemyManager.agent.stoppingDistance + 0.5f + enemyManager.enemyCombatController.minimumAttackDistanceToPlayer;
        }

        // TODO: Fix Boolean as a parameter, very ugly
        public RunningAttack GetRunAttack(bool distanceMatters)
        {
            if (runningAttacks != null && runningAttacks.Length > 0)
            {
                if (distanceMatters == false)
                {
                    var attackDice = Random.Range(0, runningAttacks.Length);
                    return runningAttacks[attackDice];
                }

                return runningAttacks.FirstOrDefault(x => Vector3.Distance(enemyManager.transform.position, enemyManager.player.transform.position) >= x.minDistanceToAttack);
            }

            return null;
        }

    }

}
