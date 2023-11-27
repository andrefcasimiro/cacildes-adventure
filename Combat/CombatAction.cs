using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class CombatAction : MonoBehaviour
    {
        [Header("Animation Clip or String Settings")]
        [Tooltip("If set, will override the generic attack animation for this clip during this combat action")]
        public AnimationClip attackAnimationClip;
        [Tooltip("If you prefer to use an animation name instead of a clip for this combat action")]
        public string attackAnimationName;

        [Header("Conditions")]
        [Range(1, 100)]
        public int minimumHealthToUse = 100;

        public float minimumDistanceToTarget = 0f;
        public float maximumDistanceToTarget = 15f;

        [Header("Events")]
        public UnityEvent onAttack_Start;
        public UnityEvent onAttack_HitboxOpen;
        public UnityEvent onAttack_End;

        [Header("Damage")]
        public Damage damage;

        [Header("Cooldowns")]
        public float maxCooldown = 15f;

        [Header("Frequency")]
        [Range(0, 1)] public float frequency = 0.5f;


        [Header("Components")]
        public CharacterManager characterManager;

        public bool CanUseCombatAction()
        {
            if (characterManager.IsBusy())
            {
                return false;
            }

            if (characterManager.characterCombatController.usedCombatActions.Contains(this))
            {
                return false;
            }

            float currentHealthPercentage = characterManager.health.GetCurrentHealthPercentage();
            if (
                currentHealthPercentage <= 0
                || currentHealthPercentage > minimumHealthToUse
            )
            {
                return false;
            }

            if (characterManager.targetManager.currentTarget != null)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, characterManager.targetManager.currentTarget.transform.position);
                if (distanceToPlayer > maximumDistanceToTarget || distanceToPlayer < minimumDistanceToTarget)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
