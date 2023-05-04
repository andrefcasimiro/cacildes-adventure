using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(SphereCollider))]
    public class ParticleInvisibleHelper : MonoBehaviour
    {
        ParticleSystem particleSystem => GetComponent<ParticleSystem>();

        void OnBecameVisible()
        {
            particleSystem.Play();
        }

        void OnBecameInvisible()
        {
            particleSystem.Stop();

            particleSystem.Play();
        }

    }
}
