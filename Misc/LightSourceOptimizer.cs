using UnityEngine;

namespace AF
{
    public class LightSourceOptimizer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GetComponent<Light>().enabled = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GetComponent<Light>().enabled = false;
            }
        }
    }
}
