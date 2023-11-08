using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(SphereCollider))]
    public class ParticleInvisibleHelper : MonoBehaviour
    {
        ParticleSystem particleSystem => GetComponent<ParticleSystem>();

        void OnBecameVisible()
        {
            if (particleSystem == null)
            {
                return;
            }

            particleSystem.Play();
        }

        void OnBecameInvisible()
        {
            if (particleSystem == null)
            {
                return;
            }

            particleSystem.Stop();

            particleSystem.Play();
        }

    }
}
