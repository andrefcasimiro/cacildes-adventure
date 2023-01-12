using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DamagePlayerOnTriggerStay : MonoBehaviour
    {
        public int damage = 50;

        public float maxCooldownBeforeAnotherDamage = 2f;
        float cooldown = Mathf.Infinity;

        PlayerHealthbox playerHealthbox;
        public ParticleSystem part => GetComponent<ParticleSystem>();

        public List<ParticleCollisionEvent> collisionEvents;

        public bool deactivateOnCollision = false;

        private void Start()
        {
            collisionEvents = new List<ParticleCollisionEvent>();

            playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
        }

        private void OnEnable()
        {
        }

        private void Update()
        {
            if (cooldown <= maxCooldownBeforeAnotherDamage)
            {
                cooldown += Time.deltaTime;
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (cooldown < maxCooldownBeforeAnotherDamage)
            {
                return;
            }
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            int i = 0;
            while (i < numCollisionEvents)
            {

                if (other.gameObject.tag == "Player" || other.gameObject.tag == "PlayerHealthbox")
                {
                    playerHealthbox.TakeEnvironmentalDamage(damage);
                    cooldown = 0f;

                    if (deactivateOnCollision)
                    {
                        break;
                    }
                }

                i++;
            }
        }

    }

}
