using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF {
    public class EnemyAttack : StateMachineBehaviour
    {
        public float attackPointsBonus = 0f;

        public StatusEffect statusEffect;
        public float amountOfStatusApplied = 20;

        public float blockStaminaBonusCost = 0;

        private Enemy enemy;
        private EnemyCombatController enemyCombatController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyCombatController == null)
            {
                enemyCombatController = enemy.GetComponent<EnemyCombatController>();
            }

            enemyCombatController.weaponDamage += attackPointsBonus;
            enemyCombatController.statusEffectAmount = amountOfStatusApplied;
            enemyCombatController.weaponStatusEffect = statusEffect;
            enemyCombatController.bonusBlockStaminaCost = blockStaminaBonusCost;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemyCombatController.weaponDamage -= attackPointsBonus;
            enemyCombatController.statusEffectAmount = 0f;
            enemyCombatController.weaponStatusEffect = null;
            enemyCombatController.bonusBlockStaminaCost = 0f;
        }
    }
}
