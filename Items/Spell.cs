using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Spell / New Spell")]
    public class Spell : Item
    {
        [Header("Spell Cast Particle")]
        public Projectile spellCastParticle;

        [Header("On Hit Settings")]
        public DestroyableParticle impactFx;
        public WeaponElementType spellElement;
        public float damageOnHitEnemy = 100;
        public StatusEffect statusEffectInflict;
        public float statusEffectInflictAmount = -1f;

        [Header("Spell Poise")]
        public int spellPoise;
        public int spellPostureDamage;


        [Header("Negative Effects Upon Usage")]
        public float selfDamageAmount = -1f;
        public WeaponElementType selfDamageType;
        public StatusEffect selfStatusEffect;
        public float selfStatusEffectAmount = -1f;

        [Header("Healing")]
        public float healingAmount = -1f;

        [Header("Reputation")]
        public bool increaseDamageWithReputation = false;


    }

}
