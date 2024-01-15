
using System.Collections.Generic;
using AF.Combat;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

namespace AF.Detection
{
    public class Sight : MonoBehaviour
    {
        public float fieldOfViewAngle = 90f;
        public float viewDistance = 10f;
        public LayerMask targetLayer;

        [Header("Components")]
        public Transform origin;
        public TargetManager targetManager;

        [Header("Tags")]
        public List<string> tagsToDetect = new();

        [Header("Events")]
        public UnityEvent OnTargetSighted;

        [Header("Settings")]
        public bool debug = false;

        [Header("Flags")]
        [SerializeField] bool isSighted = false;
        public bool canSight = true;

        public Transform IsTargetInSight()
        {
            Vector3 _origin = origin.transform.position;

            // Extend the direction vector by a factor to reach further in front of the enemy
            Vector3 direction = origin.transform.forward;
            direction *= viewDistance; // Extend the direction

            if (Physics.Raycast(_origin, direction, out RaycastHit hit, viewDistance, targetLayer))
            {
                if (debug) Debug.DrawLine(_origin, hit.point, Color.red); // Draw a red line for the raycast
                return hit.transform;
            }

            if (debug) Debug.DrawRay(_origin, direction, Color.green);
            return null;
        }

        public void CastSight()
        {
            if (canSight == false)
            {
                return;
            }

            Transform hit = IsTargetInSight();

            if (hit != null)
            {
                // Check if the hit object's tag is in the list of tags to detect

                if (tagsToDetect.Count > 0)
                {
                    isSighted = tagsToDetect.Contains(hit.transform.gameObject.tag);
                }

                if (isSighted && hit.TryGetComponent(out CharacterBaseManager target))
                {
                    targetManager.SetTarget(target);

                    OnTargetSighted?.Invoke();
                }
            }
        }

        public void SetDetectionLayer(string layerName)
        {
            this.targetLayer = LayerMask.GetMask(layerName);
        }

        public void SetTagsToDetect(List<string> tagsToDetect)
        {
            this.tagsToDetect.Clear();
            this.tagsToDetect = tagsToDetect;
        }

        public void Set_CanSight(bool value)
        {
            canSight = value;
        }
    }
}
