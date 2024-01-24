using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Spell / New Spell")]
    public class Spell : Item
    {

        [Header("Settings")]
        public Projectile spellCastParticle;
        public float costPerCast = 20;

        [Header("Animations")]
        public AnimationClip castAnimationOverride;

        [Header("Spell Type")]
        public bool isFaithSpell = false;

    }
}
