using UnityEngine;
using StarterAssets;

namespace AF
{

    public class FootstepListener : MonoBehaviour
    {
        FootstepSystem footstepSystem;

        public AudioSource footstepAudioSource;

        [Header("Jump & Gravity")]
        public AudioClip onJumpSfx;
        public AudioClip onLandSfx;
        public AudioClip landingSfx;

        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();

        private void Awake()
        {

            footstepSystem = FindObjectOfType<FootstepSystem>(true);
        }

        public void PlayLanding(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            Utils.PlaySfx(footstepAudioSource, landingSfx);
        }

        public void PlayCloth(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            Utils.PlaySfx(footstepAudioSource, onJumpSfx);
        }


        /// <summary>
        /// Animation Event
        /// </summary>
        public void PlayFootstep(AnimationEvent animationEvent)
        {
            // Avoid blends so we don't play double sounds if running and walking with blend at the same time
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            string groundTag = GetGroundTag();
            if (groundTag == null)
            {
                return;
            }

            if (!thirdPersonController.Grounded)
            {
                return;
            }

            AudioClip clip = footstepSystem.GetFootstepClip(groundTag);
            Utils.PlaySfx(footstepAudioSource, clip);
        }

        private string GetGroundTag()
        {
            RaycastHit hit;
            if (Physics.Raycast(footstepAudioSource.transform.position, -Vector3.up, out hit))
            {
                if (hit.transform != null)
                {
                    return hit.transform.gameObject.tag;
                }
            }

            return null;
        }
    }
}
