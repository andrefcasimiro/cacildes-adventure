using UnityEngine;

namespace AF
{
    public class EventNavigator : MonoBehaviour
    {
        public LayerMask eventNavigatorCapturableLayer;
        public int raycastDistance = 5;

        UIDocumentKeyPrompt documentKeyPrompt => FindObjectOfType<UIDocumentKeyPrompt>(true);
        StarterAssets.StarterAssetsInputs inputs => FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);
        RaycastHit HitInfo;

        private void Update()
        {
            Transform cameraTransform = Camera.main.transform;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out HitInfo, raycastDistance, eventNavigatorCapturableLayer))
            {
                IEventNavigatorCapturable eventNavigatorCapturable = HitInfo.collider.GetComponentInParent<IEventNavigatorCapturable>();

                if (eventNavigatorCapturable != null)
                {   
                    if (inputs.interact)
                    {
                        eventNavigatorCapturable.OnInvoked();
                    }
                    else
                    {
                        eventNavigatorCapturable.OnCaptured();
                        return;
                    }
                }
            }

            // Failed to capture anything, deactivate key prompt
            documentKeyPrompt.gameObject.SetActive(false);
        }
    }
}
