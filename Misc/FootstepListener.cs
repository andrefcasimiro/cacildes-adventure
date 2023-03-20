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
        LockOnManager lockOnManager;

        float maxCooldown = 0.1f;
        float cooldown = Mathf.Infinity;

        private void Awake()
        {
            lockOnManager = FindObjectOfType<LockOnManager>(true);
            footstepSystem = FindObjectOfType<FootstepSystem>(true);
        }

        private void Update()
        {
            if (cooldown < maxCooldown)
            {
                cooldown += Time.deltaTime;
            }
        }

        public void PlayLanding(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            BGMManager.instance.PlaySound(landingSfx, thirdPersonController.jumpAndDodgeAudiosource);
        }

        public void PlayCloth(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight <= 0.5f)
            {
                return;
            }

            BGMManager.instance.PlaySound(onJumpSfx, thirdPersonController.jumpAndDodgeAudiosource);
        }


        /// <summary>
        /// Animation Event
        /// </summary>
        public void PlayFootstep(AnimationEvent animationEvent)
        {
            if (cooldown < maxCooldown) { return; }

            bool bypassWeight = false;
         
            // Strafe diagonally edge-case
            if (thirdPersonController != null)
            {
                // Is Player
                if (lockOnManager.isLockedOn)
                {
                    if (thirdPersonController._input.move.x != 0 && thirdPersonController._input.move.y != 0)
                    {
                        bypassWeight = true;
                    }
                }
            }

            // Avoid blends so we don't play double sounds if running and walking with blend at the same time
            if (animationEvent.animatorClipInfo.weight <= 0.5f && bypassWeight == false)
            {
                return;
            }

            string groundTag = GetGroundTag();
            if (groundTag == null)
            {
                return;
            }

            if (thirdPersonController != null && thirdPersonController.Grounded == false)
            {
                return;
            }

            AudioClip clip = footstepSystem.GetFootstepClip(groundTag);
            BGMManager.instance.PlaySound(clip, footstepAudioSource);
            cooldown = 0f;
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
