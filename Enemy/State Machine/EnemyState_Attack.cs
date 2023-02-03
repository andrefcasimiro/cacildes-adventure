using UnityEngine;

namespace AF {
    public class EnemyState_Attack : StateMachineBehaviour
    {
        public int attackPointsBonus = 0;

        public StatusEffect statusEffect;
        public float amountOfStatusApplied = 20;

        public float blockStaminaBonusCost = 0;

        public int postureDamageBonus = 0;

        private EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.enemyCombatController.weaponDamage += attackPointsBonus;
            enemy.enemyCombatController.statusEffectAmount = amountOfStatusApplied;
            enemy.enemyCombatController.weaponStatusEffect = statusEffect;
            enemy.enemyCombatController.bonusBlockStaminaCost = blockStaminaBonusCost;
            enemy.enemyCombatController.currentAttackPoiseDamage += postureDamageBonus;

            // Enable tracking
            enemy.facePlayer = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.enemyCombatController.weaponDamage -= attackPointsBonus;
            enemy.enemyCombatController.statusEffectAmount = 0f;
            enemy.enemyCombatController.weaponStatusEffect = null;
            enemy.enemyCombatController.bonusBlockStaminaCost = 0f;
            enemy.enemyCombatController.currentAttackPoiseDamage -= postureDamageBonus;
        }
    }
}
