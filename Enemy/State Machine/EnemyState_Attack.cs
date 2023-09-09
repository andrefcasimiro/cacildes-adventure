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

        public bool isParriable = true;

        public bool dontFacePlayerOnStateUpdate = false;

        public bool activateHyperArmor = false;

        [Header("Execution Attack")]
        public string transitionToExecution = "";
        public string playerExecutedClip = "";
        
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

            if (activateHyperArmor)
            {
                enemy.enemyCombatController.hasHyperArmorActive = true;
            }

            // Enable tracking
            enemy.facePlayer = true;

            if (enemy.enemyPostureController != null)
            {
                enemy.enemyPostureController.isParriable = isParriable;
            }

            enemy.enemyCombatController.transitionToExecution = transitionToExecution;
            enemy.enemyCombatController.playerExecutedClip = playerExecutedClip;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.facePlayer && dontFacePlayerOnStateUpdate)
            {
                enemy.facePlayer = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            enemy.enemyCombatController.weaponDamage -= attackPointsBonus;
            enemy.enemyCombatController.statusEffectAmount = 0f;
            enemy.enemyCombatController.weaponStatusEffect = null;
            enemy.enemyCombatController.bonusBlockStaminaCost = 0f;
            enemy.enemyCombatController.currentAttackPoiseDamage -= postureDamageBonus;

            enemy.enemyCombatController.transitionToExecution = "";
            enemy.enemyCombatController.playerExecutedClip = "";
        }
    }
}
