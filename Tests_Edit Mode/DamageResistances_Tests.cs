using UnityEngine;
using AF;
using NUnit.Framework;
using AF.Health;

public class DamageResistances_Tests : MonoBehaviour
{
    [Test]
    public void FilterIncomingDamage_ResistantToPierceWeapon_ReturnsReducedPhysicalDamage()
    {
        DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();


        DamageResistances.WeaponTypeResistance weaponTypeResistance = new()
        {
            weaponAttackType = WeaponAttackType.Pierce,
            damageResistance = .5f,
            damageWeakness = 1
        };

        damageResistances.weaponTypeResistances = new DamageResistances.WeaponTypeResistance[] { weaponTypeResistance };

        Damage incomingDamage = new Damage { physical = 100, weaponAttackType = WeaponAttackType.Pierce };

        Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
        Assert.AreEqual(50, filteredDamage.physical);
    }

    [Test]
    public void FilterIncomingDamage_WeakToBluntWeapon_ReturnsLargerPhysicalDamage()
    {
        DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();

        DamageResistances.WeaponTypeResistance weaponTypeResistance = new()
        {
            weaponAttackType = WeaponAttackType.Blunt,
            damageResistance = 1f,
            damageWeakness = 2
        };

        damageResistances.weaponTypeResistances = new DamageResistances.WeaponTypeResistance[] { weaponTypeResistance };

        Damage incomingDamage = new Damage { physical = 100, weaponAttackType = WeaponAttackType.Blunt };

        Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
        Assert.AreEqual(200, filteredDamage.physical);
    }

    [Test]
    public void FilterIncomingDamage_WeakToBluntWeapon_ReturnsSamePhysicalDamageForAttackWithPierceWeapon()
    {
        DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();

        DamageResistances.WeaponTypeResistance weaponTypeResistance = new()
        {
            weaponAttackType = WeaponAttackType.Blunt,
            damageResistance = 1f,
            damageWeakness = 2
        };

        damageResistances.weaponTypeResistances = new DamageResistances.WeaponTypeResistance[] { weaponTypeResistance };

        Damage incomingDamage = new Damage { physical = 100, weaponAttackType = WeaponAttackType.Pierce };

        Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
        Assert.AreEqual(100, filteredDamage.physical);
    }

    [Test]
    public void FilterIncomingDamage_WeakToFire_ReturnsLargerFireDamage()
    {
        DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();
        damageResistances.fireDamageBonus = 2;

        Damage incomingDamage = new Damage { fire = 100 };

        Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
        Assert.AreEqual(200, filteredDamage.fire);
    }

    [Test]
    public void FilterIncomingDamage_ResistantToFire_ReturnsReducedFireDamage()
    {
        DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();
        damageResistances.fireDamageFilter = .5f;

        Damage incomingDamage = new Damage { fire = 100 };

        Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
        Assert.AreEqual(50, filteredDamage.fire);
    }
}
