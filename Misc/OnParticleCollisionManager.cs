using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class OnParticleCollisionManager : MonoBehaviour
    {
        public float maxCooldownBeforeAnotherDamage = 1f;
        float cooldown = Mathf.Infinity;

        ParticleSystem part => GetComponent<ParticleSystem>();

        public List<ParticleCollisionEvent> collisionEvents = new();

        public UnityEvent onParticleCollisionEvent;

        public string tagToCollideWith = "Player";

        DestroyableSpellParticle destroyableSpellParticle;


        private void Awake()
        {
            destroyableSpellParticle = GetComponentInParent<DestroyableSpellParticle>();
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

            if (other.CompareTag(tagToCollideWith) == false)
            {
                return;
            }

            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            if (numCollisionEvents > 0)
            {
                onParticleCollisionEvent.Invoke();

                if (destroyableSpellParticle != null)
                {
                    destroyableSpellParticle.OnCollide(other);
                }

                cooldown = 0f;
            }

        }
    }

}
