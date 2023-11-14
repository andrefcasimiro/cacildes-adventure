using UnityEngine;

namespace AF
{
    public class EventNavigator : MonoBehaviour
    {
        [Header("Layer Options")]
        public LayerMask eventNavigatorCapturableLayer;

        [Header("Settings")]
        public int raycastDistance = 5;

        [Header("Components")]
        public Transform playerTransform;
        public UIManager uiManager;

        // Internal
        IEventNavigatorCapturable currentTarget;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnInteract()
        {
            currentTarget?.OnInvoked();
        }

        private void Update()
        {
            bool hitSomething = Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward, out var hitInfo, raycastDistance, eventNavigatorCapturableLayer);

            if (hitSomething)
            {
                var eventNavigatorCapturable = hitInfo.collider.GetComponent<IEventNavigatorCapturable>();

                if (eventNavigatorCapturable != currentTarget)
                {
                    var hitPosition = hitInfo.transform.position;
                    hitPosition.y = playerTransform.transform.position.y;
                    Vector3 dist = hitPosition - playerTransform.transform.position;
                    float angle = Vector3.Angle(dist.normalized, playerTransform.transform.forward);

                    if (angle / 2 <= 50)
                    {
                        currentTarget = eventNavigatorCapturable;

                        if (uiManager.CanShowGUI())
                        {
                            currentTarget.OnCaptured();
                        }
                    }
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    currentTarget?.OnReleased();
                }

                currentTarget = null;
            }
        }
    }
}
