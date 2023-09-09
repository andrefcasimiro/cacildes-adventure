using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF {
    public class GoldParticle : MonoBehaviour
    {
        ParticleSystem particleSystem => GetComponent<ParticleSystem>();

        public float speed = 0.1f;
        GameObject player;

        AudioSource audioSource => GetComponent<AudioSource>();

        float sfxCooldown = Mathf.Infinity;
        float maxSfxCooldown = 0.1f;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            var velocityOverLifetime = particleSystem.velocityOverLifetime;
            velocityOverLifetime.speedModifier = speed;
        }

        private void Update()
        {
            if (sfxCooldown <= maxSfxCooldown)
            {
                sfxCooldown += Time.deltaTime;
            }

            if (player != null)
            {
                Vector3 directionToPlayer = player.transform.position - transform.position;
                float distance = directionToPlayer.magnitude;
                Vector3 normalizedDirection = directionToPlayer.normalized;

                var velocityOverLifetime = particleSystem.velocityOverLifetime;

                velocityOverLifetime.xMultiplier = normalizedDirection.x * distance;
                velocityOverLifetime.yMultiplier = normalizedDirection.y * distance;
                velocityOverLifetime.zMultiplier = normalizedDirection.z * distance;
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (sfxCooldown < maxSfxCooldown)
            {
                return;
            }

            audioSource.Play();

            sfxCooldown = 0;
        }
    }
}
