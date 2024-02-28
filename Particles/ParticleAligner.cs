using UnityEngine;

namespace AF.Particles
{

    public class ParticleAligner : MonoBehaviour
    {
        public Transform target;

        public ParticleSystem particleSystem;

        [Header("Options")]
        public bool adjustGravityModifierBasedOnDistanceToTarget = true;
        float defaultGravityModifier;

        private void Awake()
        {
            defaultGravityModifier = particleSystem.gravityModifier;
        }

        public void Align()
        {
            if (target == null)
            {
                Debug.LogWarning("Target (player) not assigned!");
                return;
            }

            // Calculate the direction from the grenade to the player
            Vector3 directionToTarget = target.position - transform.position;

            // Rotate the grenade to face the player
            transform.rotation = Quaternion.LookRotation(directionToTarget);

            if (adjustGravityModifierBasedOnDistanceToTarget)
            {
                float gravityModifier = defaultGravityModifier / directionToTarget.magnitude;
                ParticleSystem.MainModule mainModule = particleSystem.main;
                mainModule.gravityModifier = gravityModifier;

            }
        }
    }
}
