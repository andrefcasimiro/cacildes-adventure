using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class TownOptimizer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

}
