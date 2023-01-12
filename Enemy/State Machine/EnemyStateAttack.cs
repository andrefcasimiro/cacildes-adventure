using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF {
    public class EnemyStateAttack : StateMachineBehaviour
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

            enemy.weaponDamage += attackPointsBonus;
            enemy.statusEffectAmount = amountOfStatusApplied;
            enemy.weaponStatusEffect = statusEffect;
            enemy.bonusBlockStaminaCost = blockStaminaBonusCost;
            enemy.currentPoiseDamage += postureDamageBonus;

            // Enable tracking
            enemy.facePlayer = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.weaponDamage -= attackPointsBonus;
            enemy.statusEffectAmount = 0f;
            enemy.weaponStatusEffect = null;
            enemy.bonusBlockStaminaCost = 0f;
            enemy.currentPoiseDamage -= postureDamageBonus;
        }
    }
}
