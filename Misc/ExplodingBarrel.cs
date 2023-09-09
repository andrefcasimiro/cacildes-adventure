using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ExplodingBarrel : MonoBehaviour
    {
        MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
        MeshCollider meshCollider => GetComponent<MeshCollider>();

        public GameObject explosionFx;

        public void Explode()
        {
            explosionFx.SetActive(true);
            StartCoroutine(HideStuff());
        }

        IEnumerator HideStuff()
        {
            yield return new WaitForSeconds(0.1f);

            meshRenderer.enabled = false;
            meshCollider.enabled = false;

            var lockOnRef = GetComponentInChildren<LockOnRef>();
            if (lockOnRef != null)
            {
                lockOnRef.gameObject.SetActive(false);
            }
        }

    }

}
