
using System.Collections.Generic;
using System.Linq;
using AF.Characters;
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

        [Header("Factions")]
        public List<CharacterFaction> factionsToIgnore = new();

        [Header("Events")]
        public UnityEvent OnTargetSighted;

        [Header("Settings")]
        public bool debug = false;

        [Header("Flags")]
        [SerializeField] bool isSighted = false;
        public bool canSight = true;

        public Transform IsTargetInSight()
        {
            Vector3 originPosition = origin.transform.position;

            Vector3 direction = origin.transform.forward * viewDistance;

            // Perform the raycast
            if (Physics.Raycast(originPosition, direction, out RaycastHit hit, viewDistance, targetLayer))
            {
                if (debug) Debug.DrawLine(originPosition, hit.point, Color.red); // Draw a red line for the raycast
                return hit.transform;
            }

            // Draw a green debug line if no target is hit
            if (debug) Debug.DrawRay(originPosition, direction, Color.green);
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
                    if (factionsToIgnore == null || factionsToIgnore.Count == 0 || !target.characterFactions.Any(faction => factionsToIgnore.Contains(faction)))
                    {
                        targetManager.SetTarget(target, () =>
                        {
                            OnTargetSighted?.Invoke();
                        }, false);
                    }
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
