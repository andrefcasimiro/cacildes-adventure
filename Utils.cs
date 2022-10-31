using UnityEngine;

namespace AF
{

    public static class Utils
    {

        public static void FaceTarget(Transform origin, Transform target)
        {
            var lookPos = target.position - origin.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            origin.rotation = rotation;
        }

        public static bool PlayerIsSighted(Enemy enemy, GameObject player, LayerMask obstructionMask)
        {

            return false;
        }

        public static void PlaySfx(AudioSource source, AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            float pitch = Random.Range(0.99f, 1.01f);
            source.pitch = pitch;

            source.PlayOneShot(clip);
        }

    }

}
