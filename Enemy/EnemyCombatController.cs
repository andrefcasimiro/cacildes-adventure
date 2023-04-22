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
            Block,
            UseBuff,
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

        [Header("Override Animator Combat Attacks")]
        public string[] meleeLightAttacks;
        public string[] meleeHeavyAttacks;

        [Header("Distance To Player")]
        public float minimumAttackDistanceToPlayer = 0;

        // Stats affected by animations
        [HideInInspector] public int weaponDamage = 100;
        [HideInInspector] public int weaponDamageBonus = 0;
        [HideInInspector] public StatusEffect weaponStatusEffect = null;
        [HideInInspector] public float statusEffectAmount = 0f;
        [HideInInspector] public float bonusBlockStaminaCost = 0f;
        [HideInInspector] public int currentAttackPoiseDamage = 1;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

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
            return Player.instance.CalculateAIAttack(enemyManager.enemy.basePhysicalAttack, enemyManager.currentLevel) + weaponDamageBonus;
        }

        public bool IsInCombat()
        {
            return enemyManager.animator.GetBool(enemyManager.hashIsInCombat);
        }

        public bool IsPlayerFarAway()
        {
            return Vector3.Distance(enemyManager.agent.transform.position, enemyManager.player.transform.position) > enemyManager.agent.stoppingDistance + 0.5f + enemyManager.enemyCombatController.minimumAttackDistanceToPlayer;
        }

    }

}
