using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EventNavigator : MonoBehaviour
    {

        Ray RayOrigin;
        RaycastHit HitInfo;

        public LayerMask eventLayer;

        public int raycastDistance = 5;

        UIDocumentEventKeyPrompt documentKeyPrompt;

        StarterAssets.StarterAssetsInputs inputs;

        void Start()
        {
            documentKeyPrompt = FindObjectOfType<UIDocumentEventKeyPrompt>(true);
            inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);
        }

        private void Update()
        {
            Transform cameraTransform = Camera.main.transform;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out HitInfo, raycastDistance, eventLayer))
            {

                if (HitInfo.collider.gameObject != null)
                {
                    var targetEventPage = HitInfo.collider.GetComponent<EventPage>();
                    if (targetEventPage == null)
                    {
                        targetEventPage = HitInfo.collider.GetComponentInParent<EventPage>();
                    }

                    if (targetEventPage != null && targetEventPage.eventTrigger != EventTrigger.NO_TRIGGER)
                    {
                        HandleEvent(targetEventPage);
                    }
                }
            }
            else
            {
                documentKeyPrompt.gameObject.SetActive(false);
            }
        }

        void HandleEvent(EventPage eventPage)
        {
            if (eventPage.IsRunning() || eventPage.eventTrigger != EventTrigger.ON_KEY_PRESS)
            {
                return;
            }

            documentKeyPrompt.key = "E";
            documentKeyPrompt.action = eventPage.notificationText;

            documentKeyPrompt.gameObject.SetActive(true);

            if (inputs.interact)
            {
                eventPage.BeginEvent();
                documentKeyPrompt.gameObject.SetActive(false);
            }

        }
    }

}