using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF.Footsteps
{
    public class FootstepReceiver : MonoBehaviour
    {

        [Header("Effect Instances")]
        [SerializedDictionary("Tag", "FX Game Object")]
        public SerializedDictionary<string, GameObject> footstepEffectsDictionary = new();

        [Header("Raycast Settings")]
        public Transform transformRef;
        public LayerMask detectionLayer;
        public float rayDistanceDownwards = 0.5f;

        [Header("Cooldown Settings")]
        public float footstepCooldown = 0.25f;
        bool isTriggering = false;


        public void Trigger()
        {
            if (isTriggering)
            {
                return;
            }

            if (Physics.Raycast(transformRef.position, Vector3.down, out RaycastHit hit, rayDistanceDownwards, detectionLayer))
            {
                if (hit.transform != null && footstepEffectsDictionary.ContainsKey(hit.transform.gameObject.tag))
                {
                    isTriggering = true;

                    footstepEffectsDictionary[hit.transform.gameObject.tag].SetActive(false);
                    footstepEffectsDictionary[hit.transform.gameObject.tag].SetActive(true);

                    Invoke(nameof(ResetIsTriggering), footstepCooldown);
                }
            }
        }

        void ResetIsTriggering()
        {
            isTriggering = false;
        }
    }
}
