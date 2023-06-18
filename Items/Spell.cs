using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Spell / New Spell")]
    public class Spell : Item
    {
        [Header("Loopable Spells")]
        [Tooltip("If used, will keep the animation and the spell in loop for the duration set. Applicable to spells that require middle animation loops")]
        public float spellDuration = -1f;

        [Header("Animations")]
        public string startAnimation;

        [Header("Prepare Particle")]
        public DestroyableParticle prepareSpellParticle;
        public bool prepareSpellParticleSpawnAtChest = false;
        public bool prepareSpellParticleSpawnAtLeftHand = false;
        public bool prepareSpellParticleSpawnAtRightHand = false;
        public bool prepareSpellParticleSpawnAtFeet = false;

        [Header("Spell Cast Particle")]
        public DestroyableSpellParticle spellCastParticle;
        public bool spellCastSpawnAtChest = false;
        public bool spellCastSpawnAtLeftHand = false;
        public bool spellCastSpawnAtRightHand = false;
        public bool spellCastSpawnAtFeet = false;

        [Header("On Hit Settings")]
        public DestroyableParticle impactFx;
        public WeaponElementType spellElement;
        public float damageOnHitEnemy = 100;
        public StatusEffect statusEffectInflict;
        public float statusEffectInflictAmount = -1f;

        [Header("Spell Origin")]
        public bool spawnAtPlayer = false;
        public bool spawnAtNearestOrLockedOnEnemy = false;

        [Header("Spell Direction")]
        public bool spawnTowardsNearestOrLockedOnEnemy = false;

        [Header("Copy effects of a consumable")]
        public Consumable[] consumables;

        [Header("Spell Poise")]
        public int spellPoise;

        [Header("Spell Direction and Reach")]
        public bool rotateToEnemyOnSpellCast = true;
        public float maxSpellDistance = 20f;


        [Header("Negative Effects Upon Usage")]
        public float selfDamageAmount = -1f;
        public WeaponElementType selfDamageType;
        public StatusEffect selfStatusEffect;
        public float selfStatusEffectAmount = -1f;

        public bool IsLoopable()
        {
            return spellDuration != -1f;
        }

    }

}
