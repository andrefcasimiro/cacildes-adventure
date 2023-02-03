using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Shield / New Shield")]
    public class Shield : Item
    {
        // Physical
        public float blockStaminaCost = 30f;


        // Defense Absorption
        [Range(0, 100f)]
        public float defenseAbsorption = 95.5f;

        // Elemental
        public float fireDefense;
        public float frostDefense;
        public float lightningDefense;

        // Mystical
        public float arcaneDefense;
        public float faithDefense;
        public float darknessDefense;

        // Scaling
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;
        public Scaling arcaneScaling = Scaling.E;
        public Scaling faithScaling = Scaling.E;
        public Scaling darknessScaling = Scaling.E;

        // Requirements
        public int strengthMinimumLevel;
        public int dexterityMinimumLevel;
        public int arcaneMinimumLevel;
        public int faithMinimumLevel;
        public int darknessMinimumLevel;

        // Weight
        public float weight;

        // Graphics
        public GameObject graphic;
        public DestroyableParticle blockFx;

        public float speedPenalty = 0f;

    }

}
