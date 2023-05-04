using System.Linq;
using UnityEngine;

namespace AF
{
    public class EventNavigator : MonoBehaviour
    {
        public LayerMask eventNavigatorCapturableLayer;
        public int raycastDistance = 5;
        public float distanceToTriggerEventNavigator = 2f;

        UIDocumentKeyPrompt documentKeyPrompt;
        StarterAssets.StarterAssetsInputs inputs;
        RaycastHit HitInfo;


        IEventNavigatorCapturable currentTarget;

        private void Awake()
        {

             documentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
             inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);
        }

        void HandleTarget()
        {
            if (inputs.interact)
            {
                if (documentKeyPrompt.isActiveAndEnabled)
                {
                    documentKeyPrompt.gameObject.SetActive(false);
                }

                // If Is Event Page, Dont Run If Another Event Is Running...
                if (HitInfo.transform.GetComponent<EventPage>() != null && FindObjectsOfType<EventPage>().FirstOrDefault(x => x.isRunning) != null)
                {
                    return;
                }

                currentTarget.OnInvoked();
            }
            // Evaluate if prompt is inactive
            else if (documentKeyPrompt.gameObject.activeSelf == false)
            {
                // If close, show notification
                /*if (Vector3.Angle(HitInfo.transform.position - playerComponentManager.transform.position, playerComponentManager.transform.forward) <= 50)
                {
                    currentTarget.OnCaptured();
                }*/

                currentTarget.OnCaptured();
            }
        }

        private void Update()
        {
            if (currentTarget == null && documentKeyPrompt.isActiveAndEnabled)
            {
                documentKeyPrompt.gameObject.SetActive(false);
            }

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, raycastDistance, eventNavigatorCapturableLayer))
            {
                IEventNavigatorCapturable eventNavigatorCapturable = HitInfo.collider.GetComponent<IEventNavigatorCapturable>();
                eventNavigatorCapturable ??= HitInfo.collider.GetComponentInParent<IEventNavigatorCapturable>();
                eventNavigatorCapturable ??= HitInfo.collider.GetComponentInChildren<IEventNavigatorCapturable>();

                if (eventNavigatorCapturable != null)
                {
                    currentTarget = eventNavigatorCapturable;
                }
            }
            else
            {
                currentTarget = null;
            }


            if (currentTarget != null)
            {
                HandleTarget();
                return;
            }

        }
    }
}
