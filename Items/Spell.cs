using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Spell / New Spell")]
    public class Spell : Item
    {

        public GameObject projectile;

        public float costPerCast = 20;

        [Header("Animations")]
        public AnimationClip castAnimationOverride;

        [Header("Spell Type")]
        public bool isFaithSpell = false;

        [Header("Status Effects")]
        public StatusEffect[] statusEffects;
        public float effectsDurationInSeconds = 15f;

        [Header("Spawn Options")]
        public bool spawnAtPlayerFeet = false;
        public float playerFeetOffsetY = 0f;
        public bool spawnOnLockedOnEnemies = false;
        public bool ignoreSpawnFromCamera = false;
        public bool parentToPlayer = false;

        public string GetFormattedAppliedStatusEffects()
        {
            string result = "";

            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect != null)
                {
                    result += $"{statusEffect.name}\n";
                }
            }

            return result.TrimEnd();
        }

    }
}
