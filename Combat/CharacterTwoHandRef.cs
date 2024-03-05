using UnityEngine;
namespace AF
{

    public class CharacterTwoHandRef : MonoBehaviour
    {

        [Header("Player Settings")]
        public bool useTwoHandingTransform = true;
        public Vector3 twoHandingPosition;
        public Vector3 twoHandingRotation;
        Vector3 originalPosition;
        Quaternion originalRotation;

        private void Awake()
        {
            this.originalPosition = transform.localPosition;
            this.originalRotation = transform.localRotation;
        }

        public void UseDefaultTransform()
        {
            this.transform.localPosition = originalPosition;
            this.transform.localRotation = originalRotation;
        }

        public void UseTwoHandTransform()
        {
            if (useTwoHandingTransform == false)
            {
                return;
            }

            this.transform.localPosition = twoHandingPosition;
            this.transform.localEulerAngles = twoHandingRotation;
        }

    }
}
