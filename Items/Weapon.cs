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

    [CreateAssetMenu(menuName = "Weapon / New Weapon")]
    public class Weapon : Item
    {
        // Physical
        public float physicalAttack;

        // Elemental
        public float fireAttack;
        public float frostAttack;
        public float lightningAttack;

        // Mystical
        public float arcaneAttack;
        public float faithAttack;
        public float darknessAttack;

        // Buildups
        public float bleedBuildup;
        public float poisonBuildup;

        // Scaling
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;
        public Scaling arcaneScaling = Scaling.E;
        public Scaling faithScaling = Scaling.E;
        public Scaling darknessScaling = Scaling.E;

        // Requirements
        public int strengthMinimumLevel;
        public int dexterityMinimumLevel;
        public int arcaneMinimumLevel;
        public int faithMinimumLevel;
        public int darknessMinimumLevel;

        // Weight
        public float weight;

        // Graphics
        public GameObject graphic;

        // SFX
        public AudioClip swingSfx;
        public AudioClip impactSfx;

        // Animator Overrider
        public AnimatorOverrideController animatorOverrideController;

    }

}
