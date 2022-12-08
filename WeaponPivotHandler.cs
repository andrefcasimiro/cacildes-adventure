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

        [Header("Back Ref Transform")]
        public bool useCustomBackRefTransform = false;

        public Vector3 backPosition;
        public float backRotationX;
        public float backRotationY;
        public float backRotationZ;


        private void Awake()
        {
            originalPosition = this.transform.localPosition;
            originalRotation = this.transform.localRotation;
        }

    }

}
