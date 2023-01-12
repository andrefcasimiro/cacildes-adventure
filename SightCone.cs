using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class SightCone : MonoBehaviour
    {
        public bool playerWithinRange = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                playerWithinRange = true;
            }   
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                playerWithinRange = false;
            }
        }

    }

}
