using UnityEngine;

namespace AF
{

    public class DeactivateRendererOnStart : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

}
