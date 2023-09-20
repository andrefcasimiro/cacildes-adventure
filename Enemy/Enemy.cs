using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public enum EnemySize
    {
        Small = 1,
        Medium = 2,
        Large = 3,
        Collossal = 4,
    }

    [CreateAssetMenu(menuName = "NPCs / Enemy / New Enemy")]
    public class Enemy : ScriptableObject
    {
        [Header("Generic")]
        public new LocalizedText name;
        public bool isMale = true;
        public EnemySize weight = EnemySize.Small;

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
        [Range(1, 5f)] public float fireDamageBonus = 1;
        [Range(1, 5f)] public float frostDamageBonus = 1;
        [Range(1, 5f)] public float lightningDamageBonus = 1;
        [Range(1, 5f)] public float magicDamageBonus = 1;
        [Range(1, 5f)] public float darknessDamageBonus = 1;

        [Header("Negative Status Resistance")]
        public NegativeStatusResistance[] negativeStatusResistances;

        [Header("Posture")]
        public int brokenPostureDamageMultiplier = 3;
        public int postureDamagePerParry = 25;
        public int maxPostureDamage = 100;
        public float postureDecreaseRate = 1.25f;

        [Header("EXP & Loot")]
        public int baseGold = 500;

        public List<EnemyLoot.DropCurrency> lootTable = new();


        [Header("Companions")]
        public LocalizedText hugoCommentsOnKillingEnemy;
        public LocalizedText balbinoCommentsOnKillingEnemy;
        public LocalizedText alcinoCommentsUponKillingEnemy;
    }
}
