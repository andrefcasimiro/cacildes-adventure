namespace AF.Health
{

    [System.Serializable]
    public class StatusEffectEntry
    {
        public StatusEffect statusEffect;
        public float amountPerHit;
    }

    [System.Serializable]
    public enum DamageType
    {
        NORMAL,
        COUNTER_ATTACK,
        ENRAGED,
    }

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

        public StatusEffectEntry[] statusEffects;

        public bool ignoreBlocking = false;
        public bool canNotBeParried = false;
        public DamageType damageType = DamageType.NORMAL;

        public Damage()
        {
        }

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
            StatusEffectEntry[] statusEffects,
            float pushForce,
            bool ignoreBlocking,
            bool canNotBeParried)
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
            this.statusEffects = statusEffects;
            this.pushForce = pushForce;
            this.ignoreBlocking = ignoreBlocking;
            this.canNotBeParried = canNotBeParried;
        }

        public int GetTotalDamage()
        {
            return physical + fire + frost + magic + lightning + darkness;
        }

        public void ScaleDamage(float multiplier)
        {
            this.physical = (int)(this.physical * multiplier);
            this.fire = (int)(this.fire * multiplier);
            this.frost = (int)(this.frost * multiplier);
            this.magic = (int)(this.magic * multiplier);
            this.lightning = (int)(this.lightning * multiplier);
            this.darkness = (int)(this.darkness * multiplier);
        }

        public void ScaleSpell(AttackStatManager attackStatManager, Weapon currentWeapon, int playerReputation, bool isFaithSpell)
        {
            if (this.fire > 0)
            {
                this.fire += (int)(currentWeapon.GetWeaponFireAttack() + attackStatManager.GetIntelligenceBonusFromWeapon(currentWeapon));
            }

            if (this.frost > 0)
            {
                this.frost += (int)(currentWeapon.GetWeaponFrostAttack() + attackStatManager.GetIntelligenceBonusFromWeapon(currentWeapon));
            }

            if (this.magic > 0)
            {
                this.magic += (int)(currentWeapon.GetWeaponMagicAttack() + attackStatManager.GetIntelligenceBonusFromWeapon(currentWeapon));
            }

            if (this.lightning > 0)
            {
                this.lightning += (int)(currentWeapon.GetWeaponLightningAttack() + attackStatManager.GetIntelligenceBonusFromWeapon(currentWeapon));
            }

            if (this.darkness > 0)
            {
                this.darkness += (int)(currentWeapon.GetWeaponDarknessAttack() + attackStatManager.GetIntelligenceBonusFromWeapon(currentWeapon));
            }

            if (this.pushForce > 0 && isFaithSpell)
            {
                this.pushForce += playerReputation > 0 ? (int)(playerReputation * 1.25f) : 0;
            }
        }

        public void ScaleProjectile(AttackStatManager attackStatManager, Weapon currentWeapon)
        {
            // Steel arrow might inherit magic from a magical bow, hence don't check if base values are greater than zero
            this.physical += (int)(currentWeapon.GetWeaponAttack() + attackStatManager.GetDexterityBonusFromWeapon(currentWeapon));

            if (attackStatManager.playerManager.statsBonusController.projectileMultiplierBonus > 0f)
            {
                this.physical = (int)(this.physical * attackStatManager.playerManager.statsBonusController.projectileMultiplierBonus);
            }

            this.fire += (int)currentWeapon.GetWeaponFireAttack();
            this.frost += (int)currentWeapon.GetWeaponFrostAttack();
            this.magic += (int)(currentWeapon.GetWeaponMagicAttack() + attackStatManager.GetIntelligenceBonusFromWeapon(currentWeapon));
            this.lightning += (int)currentWeapon.GetWeaponLightningAttack();
            this.darkness += (int)currentWeapon.GetWeaponDarknessAttack();
        }


        public Damage Clone()
        {
            return (Damage)this.MemberwiseClone();
        }

        public void ScaleDamageForNewGamePlus(GameSession gameSession)
        {
            this.physical = Utils.ScaleWithCurrentNewGameIteration(this.physical, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.fire = Utils.ScaleWithCurrentNewGameIteration(this.fire, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.frost = Utils.ScaleWithCurrentNewGameIteration(this.frost, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.lightning = Utils.ScaleWithCurrentNewGameIteration(this.lightning, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.magic = Utils.ScaleWithCurrentNewGameIteration(this.magic, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.darkness = Utils.ScaleWithCurrentNewGameIteration(this.darkness, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.poiseDamage = Utils.ScaleWithCurrentNewGameIteration(this.poiseDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.postureDamage = Utils.ScaleWithCurrentNewGameIteration(this.postureDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
        }
    }
}
