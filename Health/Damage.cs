namespace AF.Health
{
    [System.Serializable]
    public class Damage
    {
        public int physical;
        public int fire;
        public int frost;
        public int magic;
        public int lightning;
        public int darkness;
        public int postureDamage;
        public int poiseDamage;
        public float pushForce = 0;

        public WeaponAttackType weaponAttackType;
        public StatusEffect statusEffect;
        public float statusEffectAmount;

        public Damage(
            int physical,
            int fire,
            int frost,
            int magic,
            int lightning,
            int darkness,
            int postureDamage,
            int poiseDamage,
            WeaponAttackType weaponAttackType,
            StatusEffect statusEffect,
            float statusEffectAmount,
            float pushForce)
        {
            this.physical = physical;
            this.fire = fire;
            this.frost = frost;
            this.magic = magic;
            this.lightning = lightning;
            this.darkness = darkness;
            this.postureDamage = postureDamage;
            this.poiseDamage = poiseDamage;
            this.weaponAttackType = weaponAttackType;
            this.statusEffect = statusEffect;
            this.statusEffectAmount = statusEffectAmount;
            this.pushForce = pushForce;
        }

    }
}