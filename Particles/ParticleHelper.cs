using UnityEngine;

namespace AF
{
    public class ParticleHelper : MonoBehaviour
    {
        ParticleSystem _particleSystem => GetComponent<ParticleSystem>();

        public void SafeStop()
        {
            if (_particleSystem.isPlaying)
            {
                _particleSystem.Stop();
            }
        }
        public void SafePlay()
        {
            if (!_particleSystem.isPlaying)
            {
                _particleSystem.Play();
            }
        }
    }
}
