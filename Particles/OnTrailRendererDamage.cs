using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    // Credits: https://www.reddit.com/r/Unity3D/comments/14hpici/detecting_collisions_on_trail_renderer_in_3d/
    public class CollisionsOnTrailRenderer : MonoBehaviour
    {

        public TrailRenderer trailRenderer;
        public float detectionRange = 5.0f;

        public UnityEvent onPlayerCollision;

        private void FixedUpdate()
        {
            if (trailRenderer.enabled == false)
            {
                return;
            }

            //I dont want to do this raycasting all the time, so I am going to check if the player
            //is close enough, if not we can ignore trying to detect collisions
            if (!Physics.CheckSphere(transform.position, detectionRange, LayerMask.GetMask("TempCast")))
            {
                return;
            }

            for (int i = 0; i < trailRenderer.positionCount; i++)
            {
                if (i == trailRenderer.positionCount - 1)
                    continue;

                float t = i / (float)trailRenderer.positionCount;

                //get the approximate width of the line segment
                float width = trailRenderer.widthCurve.Evaluate(t);

                Vector3 startPosition = trailRenderer.GetPosition(i);
                Vector3 endPosition = trailRenderer.GetPosition(i + 1);
                Vector3 direction = endPosition - startPosition;
                float distance = Vector3.Distance(endPosition, startPosition);

                RaycastHit hit;

                if (Physics.SphereCast(startPosition, width, direction, out hit, distance, LayerMask.GetMask("TempCast")))
                {
                    onPlayerCollision?.Invoke();
                    return;
                }

            }
            //Physics.SphereCastAll
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

    }
}