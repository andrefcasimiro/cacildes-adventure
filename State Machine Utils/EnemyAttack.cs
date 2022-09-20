using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF {
    public class EnemyAttack : StateMachineBehaviour
    {
        public float attackPointsBonus = 0f;

        Enemy enemy;

        public StatusEffect statusEffect;
        public float amountOfStatusApplied = 20;

        public float blockStaminaBonusCost = 0;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            enemy.weaponDamage += attackPointsBonus;
            enemy.statusEffectAmount = amountOfStatusApplied;
            enemy.weaponStatusEffect = statusEffect;
            enemy.bonusBlockStaminaCost = blockStaminaBonusCost;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.weaponDamage -= attackPointsBonus;
            enemy.statusEffectAmount = 0f;
            enemy.weaponStatusEffect = null;
            enemy.bonusBlockStaminaCost = 0f;
        }
    }
}