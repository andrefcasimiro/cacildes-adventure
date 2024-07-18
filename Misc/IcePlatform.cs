using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class IcePlatform : MonoBehaviour
    {
        ThirdPersonController tps;

        public bool isExiting = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (tps == null)
                {
                    tps = other.GetComponent<ThirdPersonController>();
                }

                tps.isSliding = !isExiting;
            }
        }
    }

}
