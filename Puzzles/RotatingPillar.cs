using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Puzzles
{
    public class RotatingPillar : MonoBehaviour
    {
        public LayerMask pillarLayer;

        [Header("Settings")]
        public float rotationUnit = 1f;
        public int maxDistanceToSearchForOtherPillars = 150;

        [Header("On This Pillar Receiving Events")]
        public UnityEvent onActivate;
        public UnityEvent onDeactivate;

        [Header("Rendering")]
        public Transform activeLightShaft;

        [Header("Current Pillar Target")]
        public RotatingPillar currentTarget;

        [Header("Options")]
        public bool isActiveByDefault = false;

        Vector3 initialLightShaftScale;

        private void Awake()
        {
            initialLightShaftScale = new Vector3(this.activeLightShaft.transform.localScale.x, 0, this.activeLightShaft.transform.localScale.z);

            if (isActiveByDefault)
            {
                OnReceiveLight();
            }
            else
            {
                OnLosingLight();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnRotateInput()
        {
            if (!this.activeLightShaft.gameObject.activeSelf)
            {
                return;
            }

            transform.Rotate(Vector3.up, rotationUnit * Time.deltaTime); // Rotate the pillar smoothly

            Debug.DrawRay(transform.position + transform.up, transform.forward, Color.red);

            if (Physics.Raycast(transform.position + transform.up, transform.forward, out RaycastHit hit, maxDistanceToSearchForOtherPillars, pillarLayer))
            {
                OnPillarFound(hit.transform);
            }
            else
            {
                OnNothingFound();
            }
        }

        void OnPillarFound(Transform targetPillar)
        {
            targetPillar.TryGetComponent(out RotatingPillar rotatingPillarTarget);

            if (rotatingPillarTarget == null || rotatingPillarTarget == this || rotatingPillarTarget.IsActive())
            {
                return;
            }

            currentTarget = rotatingPillarTarget;

            currentTarget.OnReceiveLight();

            ActivateLightShaft();
        }

        void OnNothingFound()
        {
            ClearTarget();
            DeactivateLightShaft();
        }

        void ClearTarget()
        {
            if (currentTarget == null)
            {
                return;
            }

            currentTarget.OnLosingLight();
            currentTarget = null;
        }

        void ActivateLightShaft()
        {
            float distance = Vector3.Distance(currentTarget.transform.position, transform.position);
            Vector3 targetScale = initialLightShaftScale - new Vector3(0f, distance / 1.5f, 0f);
            this.activeLightShaft.transform.localScale = targetScale;
        }

        void DeactivateLightShaft()
        {
            Vector3 targetScale = initialLightShaftScale - new Vector3(0f, maxDistanceToSearchForOtherPillars, 0f);
            this.activeLightShaft.transform.localScale = targetScale;
        }

        void OnReceiveLight()
        {
            this.activeLightShaft.gameObject.SetActive(true);

            this.onActivate?.Invoke();
        }

        void OnLosingLight()
        {
            if (this.activeLightShaft != null)
            {
                this.activeLightShaft.gameObject.SetActive(false);
            }

            this.onDeactivate?.Invoke();
        }

        public bool IsActive()
        {
            return isActiveByDefault || currentTarget != null;
        }
    }
}
