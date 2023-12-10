using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

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

        public static Vector3 GetNearestNavMeshPoint(Vector3 reference)
        {
            // Teleport near player
            NavMeshHit hit;
            NavMesh.SamplePosition(reference, out hit, Mathf.Infinity, NavMesh.AllAreas);

            return hit.position;
        }

        public static void AvoidInvalidPaths(NavMeshAgent agent)
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(agent.transform.position, agent.transform.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {

                NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas);

                if (!float.IsNaN(hit.position.x) && !float.IsInfinity(hit.position.x) &&
                    !float.IsNaN(hit.position.y) && !float.IsInfinity(hit.position.y) &&
                    !float.IsNaN(hit.position.z) && !float.IsInfinity(hit.position.z))
                {
                    // It's a valid position, so assign it to nextPosition
                    agent.nextPosition = hit.position != null ? hit.position : agent.transform.position;
                    agent.updatePosition = true;
                }
                else
                {
                    // Handle the case where the position is invalid
                    Debug.LogError("Invalid positionWithLocalOffset: " + hit.position);
                }

            }
        }

        public static void UpdateTransformChildren(Transform transformTarget, bool isActive)
        {
            if (transformTarget.childCount <= 0)
            {
                return;
            }

            foreach (Transform transformChild in transformTarget)
            {
                transformChild.gameObject.SetActive(isActive);
            }
        }

    }

}
