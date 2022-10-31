using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class WeaponPivotHandler : MonoBehaviour
    {
        public Vector3 originalPosition;
        public Quaternion originalRotation;

        [Header("Blocking Transform")]
        public Vector3 blockingPosition;
        public float blockRotationX;
        public float blockRotationY;
        public float blockRotationZ;


        private void Start()
        {
            originalPosition = this.transform.localPosition;
            originalRotation = this.transform.localRotation;
        }

    }

}
