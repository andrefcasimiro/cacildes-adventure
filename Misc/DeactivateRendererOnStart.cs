using UnityEngine;

namespace AF
{

    public class DeactivateRendererOnStart : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.enabled = false;
        }
    }

}
