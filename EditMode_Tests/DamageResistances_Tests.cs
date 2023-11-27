using UnityEngine;
using NUnit.Framework;
using AF.Health;

namespace AF.Tests
{
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

            Damage incomingDamage = new(
                physical: 100, magic: 0, fire: 0, frost: 0, lightning: 0, postureDamage: 30, poiseDamage: 1, weaponAttackType: WeaponAttackType.Pierce);

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

            Damage incomingDamage = new(
                physical: 100, magic: 0, fire: 0, frost: 0, lightning: 0, postureDamage: 30, poiseDamage: 1, weaponAttackType: WeaponAttackType.Blunt);

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

            Damage incomingDamage = new(
                physical: 100, magic: 0, fire: 0, frost: 0, lightning: 0, postureDamage: 30, poiseDamage: 1, weaponAttackType: WeaponAttackType.Pierce);

            Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
            Assert.AreEqual(100, filteredDamage.physical);
        }

        [Test]
        public void FilterIncomingDamage_WeakToFire_ReturnsLargerFireDamage()
        {
            DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();
            damageResistances.fireDamageBonus = 2;

            Damage incomingDamage = new(
                physical: 0, magic: 0, fire: 100, frost: 0, lightning: 0, postureDamage: 30, poiseDamage: 1, weaponAttackType: WeaponAttackType.Pierce);

            Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
            Assert.AreEqual(200, filteredDamage.fire);
        }

        [Test]
        public void FilterIncomingDamage_ResistantToFire_ReturnsReducedFireDamage()
        {
            DamageResistances damageResistances = new GameObject().AddComponent<DamageResistances>();
            damageResistances.fireDamageFilter = .5f;

            Damage incomingDamage = new(
                physical: 0, magic: 0, fire: 100, frost: 0, lightning: 0, postureDamage: 30, poiseDamage: 1, weaponAttackType: WeaponAttackType.Pierce);

            Damage filteredDamage = damageResistances.FilterIncomingDamage(incomingDamage);
            Assert.AreEqual(50, filteredDamage.fire);
        }
    }

}