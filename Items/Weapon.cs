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

    public enum WeaponAttackType
    {
        Slash,
        Pierce,
        Blunt,
    }

    public enum WeaponElementType
    {
        None,
        Fire,
        Frost,
        Lightning,
        Magic,
    }

    [System.Serializable]
    public class WeaponStatusEffectPerHit
    {
        public StatusEffect statusEffect;
        public float amountPerHit;
    }

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon")]
    public class Weapon : Item
    {
        [Header("Attack")]
        public WeaponAttackType weaponAttackType;
        public float physicalAttack;
        public int heavyAttackBonus;

        [Header("Level & Upgrades")]
        public int level = 1;
        public float attackMultiplierPerLevel = 5f;
        public int baseGoldLevelToUpgradeWeapon = 100;

        public UpgradeMaterial upgradeMaterial;
        public float requiredOresPerLevelMultiplier = 2.25f;

        public float fireAttack;
        public float frostAttack;
        public float lightningAttack;
        public float magicAttack;

        [Header("Poise Damage")]
        public int poiseDamageBonus = 0;
        [Tooltip("How much block hit this weapon does on an enemy shield. Heavier weapons should do at least 2 or 3 hits.")] public int blockHitAmount = 1;

        [Header("Block Absorption")]
        public bool hideShield = true;
        [Range(0, 100)] public int blockAbsorption = 75;
        public int blockStaminaCost = 20;
        public float blockLayerWeight = 1f;

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
        public DestroyableParticle elementImpactFx;
        public DestroyableParticle metalImpactFx;
        public DestroyableParticle woodImpactFx;
        public DestroyableParticle waterImpactFx;
        public AnimatorOverrideController animatorOverrideController;
        public DestroyableParticle blockFx;

        [Header("Audio")]
        public AudioClip swingSfx;
        public AudioClip impactFleshSfx;

        [Header("Dual Wielding Options")]
        public bool isDualWielded = false;
        public bool halveDamage = false;

        [Header("Hide Options")]
        public bool useHolsterRef = false;
        public bool useBackRef = false;

        [Header("Speed Penalty")]
        [Tooltip("Will be added as a negative speed to the animator when equipped")]
        public float speedPenalty = 0f;

        [Header("Weapon Special Attack")]
        public GameObject weaponSpecial;
        public bool parentWeaponSpecialToPlayer = true;

        public string GetWeaponDisplayName()
        {
            return this.name.GetText() + (level > 1 ? " +" + level : "");
        }

        public string GetWeaponEnglishName()
        {
            return this.name.GetEnglishText() + (level > 1 ? " +" + level : "");
        }

        public int CalculateValue(int baseValue, int currentLevel)
        {
            var baseAttack = baseValue + (int)(currentLevel * attackMultiplierPerLevel);
            var bonusAttack = (baseValue / Mathf.Clamp(10 - currentLevel, 1, 10));
            return (int)(baseAttack + bonusAttack);
        }

        public int GetWeaponAttack()
        {
            return CalculateValue((int)physicalAttack, this.level);
        }
        public int GetWeaponAttackForLevel(int level)
        {
            return CalculateValue((int)physicalAttack, level);
        }
        public int GetWeaponFireAttack()
        {
            return CalculateValue((int)fireAttack, this.level);
        }
        public int GetWeaponFireAttackForLevel(int level)
        {
            return CalculateValue((int)fireAttack, level);
        }
        public int GetWeaponFrostAttack()
        {
            return CalculateValue((int)frostAttack, this.level);
        }
        public int GetWeaponFrostAttackForLevel(int level)
        {
            return CalculateValue((int)frostAttack, level);
        }
        public int GetWeaponLightningAttack()
        {
            return CalculateValue((int)lightningAttack, this.level);
        }
        public int GetWeaponLightningAttackForLevel(int level)
        {
            return CalculateValue((int)lightningAttack, level);
        }

        public int GetWeaponMagicAttack()
        {
            return CalculateValue((int)magicAttack, this.level);
        }
        public int GetWeaponMagicAttackForLevel(int level)
        {
            return CalculateValue((int)magicAttack, level);
        }

        public int GetRequiredOresForGivenLevel(int level)
        {
            return (int)Mathf.Floor((level / 2) * requiredOresPerLevelMultiplier);
        }

        public int GetRequiredUpgradeGoldForGivenLevel(int level)
        {
            return (int)Mathf.Floor(level * baseGoldLevelToUpgradeWeapon);
        }

    }

}
