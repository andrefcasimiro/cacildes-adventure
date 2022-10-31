using UnityEngine;

namespace AF
{
    public enum Scaling
    {
        S,
        A,
        B,
        C,
        D,
        E
    }

    public enum WeaponType
    {
        Melee,
        Staff,
        Bow,
    }

    [System.Serializable]
    public class WeaponStatusEffectPerHit
    {
        public StatusEffect statusEffect;
        public float amountPerHit;
    }

    [CreateAssetMenu(menuName = "Weapon / New Weapon")]
    public class Weapon : Item
    {
        [Header("Attack")]
        public float physicalAttack;

        public float fireAttack;
        public float frostAttack;
        public float lightningAttack;

        [Header("Poise Damage")]
        public int poiseDamageBonus = 0;

        [Header("Stamina")]
        public int lightAttackStaminaCost = 20;
        public int heavyAttackStaminaCost = 35;

        [Header("Status Effects")]
        // Status Effect
        public WeaponStatusEffectPerHit[] statusEffects;

        [Header("Scaling")]
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;

        [Header("Visual")]
        public GameObject graphic;
        public DestroyableParticle metalImpactFx;
        public AnimatorOverrideController animatorOverrideController;

        [Header("Audio")]
        public AudioClip swingSfx;
        public AudioClip impactFleshSfx;

        [Header("Dual Wielding Options")]
        public bool isDualWielded = false;

        [Header("Hide Options")]
        public bool useHolsterRef = false;
        public bool useBackRef = false;

    }

}
