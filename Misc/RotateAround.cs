using UnityEngine;

namespace AF
{
    public class RotateAround : MonoBehaviour
    {
        public float minXRotation = -45f; // Minimum X rotation angle
        public float maxXRotation = 45f;  // Maximum X rotation angle
        public float xRotationSpeed = 45f;  // Rotation speed

        private bool upwardsDirection = true;

        Quaternion originalRotation;

        private void Awake()
        {
            originalRotation = transform.rotation;
        }

        private void OnEnable()
        {
            transform.rotation = originalRotation;
        }

        void Update()
        {
            // Calculate the target rotation
            float targetX = transform.eulerAngles.x + (upwardsDirection ? 1 : -1) * Time.deltaTime * xRotationSpeed;

            // Switch direction if limits are reached
            if (targetX >= maxXRotation)
            {
                targetX = maxXRotation;
                upwardsDirection = false;
            }
            else if (targetX <= minXRotation)
            {
                targetX = minXRotation;
                upwardsDirection = true;
            }

            // Apply rotations to the transform
            transform.eulerAngles = new Vector3(targetX, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
