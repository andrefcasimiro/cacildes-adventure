
using UnityEngine;

namespace AF.Footsteps
{
    public class FootstepParticle : MonoBehaviour
    {
        public AudioClip[] clips;

        public AudioSource audioSource;

        private void OnParticleSystemStopped()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            audioSource.PlayOneShot(GetFootstepClip());
        }

        public AudioClip GetFootstepClip()
        {
            int clipDice = Random.Range(0, clips.Length);

            return clips[clipDice];
        }
    }
}