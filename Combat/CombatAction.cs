using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class CombatAction : MonoBehaviour
    {
        public AnimationClip attackAnimation;

        [Header("Conditions")]
        [Range(1, 100)]
        public int minimumHealthToUse = 100;

        public float minimumDistanceToTarget = 0f;
        public float maximumDistanceToTarget = 15f;

        [Header("Events")]
        public UnityEvent onAttack_Start;
        public UnityEvent onAttack_HitboxOpen;
        public UnityEvent onAttack_End;

        [Header("Cooldowns")]
        public float maxCooldown = 15f;

        [Header("Frequency")]
        [Range(0, 1)] public float frequency = 0.5f;


        [Header("Components")]
        public CharacterManager characterManager;

        public bool CanUseCombatAction()
        {
            if (characterManager.characterCombatController.characterManager.IsBusy())
            {
                return false;
            }

            if (characterManager.characterCombatController.usedCombatActions.Contains(this))
            {
                return false;
            }

            float currentHealthPercentage =
                1f; //characterCombatController.characterManager.enemyHealthController.currentHealth * 100 / characterCombatController.characterManager.enemyHealthController.GetMaxHealth();


            if (
                currentHealthPercentage <= 0
                || currentHealthPercentage < minimumHealthToUse
            )
            {
                return false;
            }

            if (characterManager.targetManager.CurrentTarget != null)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, characterManager.targetManager.CurrentTarget.position);
                if (distanceToPlayer > maximumDistanceToTarget || distanceToPlayer < minimumDistanceToTarget)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
