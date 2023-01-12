using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Enemy / New Enemy")]
    public class Enemy : ScriptableObject
    {
        [Header("Generic")]
        public string name;
        public bool isMale = true;


        [Header("Stats")]
        public int baseHealth = 1100;
        public int basePhysicalAttack = 90;

        [Header("Poise")]
        public int maxPoiseHits = 1;
        public float maxTimeBeforeResettingPoise = 15f;
        public float maxCooldownBeforeTakingAnotherHitToPoise = 2f;

        [Header("Sounds")]
        public AudioClip hurtSfx;
        public AudioClip deathSfx;
        public AudioClip femaleHurtSfx;
        public AudioClip femaleDeathSfx;
        public AudioClip dodgeSfx;

        [Header("Elemental Damage")]
        public int fireDamageBonus = 0;
        public int frostDamageBonus = 0;
        public int lightningDamageBonus = 0;
        public int magicDamageBonus = 0;
        public int darknessDamageBonus = 0;

        [Header("Negative Status Resistance")]
        public NegativeStatusResistance[] negativeStatusResistances;

        [Header("Posture")]
        public int brokenPostureDamageMultiplier = 3;
        public int postureDamagePerParry = 25;
        public int maxPostureDamage = 100;
        public float postureDecreaseRate = 1.25f;

        [Header("FX")]
        public DestroyableParticle damagedParticle;

        [Header("EXP & Loot")]
        public int baseGold = 500;

        public List<DropCurrency> lootTable = new List<DropCurrency>();
    }

}
