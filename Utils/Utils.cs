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

        public static bool PlayerIsSighted(Enemy enemy, Player player, LayerMask obstructionMask)
        {

            if (Vector3.Distance(enemy.transform.position, player.transform.position) > enemy.sightDistance)
            {
                return false;
            }

            Vector3 directionToTarget = (player.transform.position - enemy.transform.position);

            if (Vector3.Angle(enemy.transform.forward, directionToTarget) < enemy.fovAngle)
            {
                float distanceToTarget = Vector3.Distance(enemy.transform.position, player.transform.position);

                if (!Physics.Raycast(enemy.transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    return true;
                }
            }

            return false;
        }

        public static void PlaySfx(AudioSource source, AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            float pitch = Random.Range(0.95f, 1.05f);
            source.pitch = pitch;

            source.PlayOneShot(clip);
        }

    }

}
