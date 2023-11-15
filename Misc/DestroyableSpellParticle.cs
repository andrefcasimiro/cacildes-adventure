
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class DestroyableSpellParticle : DestroyableParticle
    {
        public Spell spell;
        [HideInInspector] public StatsBonusController statsBonusController;


        public bool collideOnlyOnce = true;

        public float timeBetweenDamage = Mathf.Infinity;
        public float maxTimeBetweenDamage = 0.1f;

        public int pushForce = 0;
        public bool isPullForce = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        private void OnParticleCollision(GameObject other)
        {
            OnCollide(other);
        }

        private void Update()
        {
            if (timeBetweenDamage < maxTimeBetweenDamage)
            {
                timeBetweenDamage += Time.deltaTime;
            }
        }

        public void OnCollide(GameObject other)
        {
            if (spell == null)
            {
                return;
            }

            if (other.CompareTag("Explodable"))
            {
                other.TryGetComponent(out ExplodingBarrel explodingBarrel);

                if (explodingBarrel != null)
                {
                    explodingBarrel.Explode();
                    return;
                }
            }


            if (other.CompareTag("Ignitable"))
            {
                other.TryGetComponent(out FireablePillar fireablePillar);

                if (fireablePillar != null)
                {
                    fireablePillar.Explode();
                    return;
                }
            }


            if (collideOnlyOnce == false && timeBetweenDamage < maxTimeBetweenDamage)
            {
                return;
            }
        }

    }
}
