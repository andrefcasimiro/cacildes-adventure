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
        public int postureDamage;
        public int poiseDamage;

        public WeaponAttackType weaponAttackType;

        public Damage(
            int physical,
            int fire,
            int frost,
            int magic,
            int lightning,
            int postureDamage,
            int poiseDamage,
            WeaponAttackType weaponAttackType)
        {
            this.physical = physical;
            this.fire = fire;
            this.frost = frost;
            this.magic = magic;
            this.lightning = lightning;
            this.postureDamage = postureDamage;
            this.poiseDamage = poiseDamage;
            this.weaponAttackType = weaponAttackType;
        }

    }
}