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

        [Header("Combo Options")]
        public AnimationClip comboClip;
        public AnimationClip comboClip2;
        public AnimationClip comboClip3;

        [Header("Conditions")]
        [Range(1, 100)]
        public int minimumHealthToUse = 100;
        public bool dontUseBelowHalfHealth = false;

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

        [Header("Settings")]
        [Range(0.1f, 2f)] public float animationSpeed = 1f;
        public bool hasHyperArmor = false;

        [Header("Components")]
        public CharacterManager characterManager;

        void Awake()
        {
            // Scale enemy damage according to new game plus
            damage.ScaleDamageForNewGamePlus(characterManager.gameSession);
        }

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

            if (dontUseBelowHalfHealth && currentHealthPercentage < 0.5f)
            {
                return false;
            }

            if (characterManager.targetManager.currentTarget != null)
            {
                var distanceToPlayer = Vector3.Distance(characterManager.transform.position, characterManager.targetManager.currentTarget.transform.position)
                    + characterManager.characterController.radius / 2;

                if (distanceToPlayer > maximumDistanceToTarget || distanceToPlayer < minimumDistanceToTarget)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
