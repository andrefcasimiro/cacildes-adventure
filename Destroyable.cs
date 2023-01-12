using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class Destroyable : MonoBehaviour
    {
        public GameObject replacement;

        Quaternion initialRotation;

        private void Awake()
        {
            this.initialRotation = transform.rotation;
            replacement.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            replacement.gameObject.SetActive(true);

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().rotation = initialRotation;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            GetComponent<MeshRenderer>().enabled = false;

            GetComponent<BoxCollider>().enabled = false;
        }
    }

}
