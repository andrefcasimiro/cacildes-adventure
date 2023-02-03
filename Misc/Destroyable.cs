using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class Destroyable : MonoBehaviour
    {
        public GameObject replacement;

        Quaternion initialRotation;

        public AudioClip onDestroySfx;

        bool isDestroyed = false;

        public BoxCollider[] collidersToDisableOnDestroy;

        private void Awake()
        {
            this.initialRotation = transform.rotation;
            replacement.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            if (isDestroyed)
            {
                return;
            }

            isDestroyed = true;

            var playerCombatController = FindObjectOfType<PlayerCombatController>(true);
            BGMManager.instance.PlaySound(onDestroySfx, playerCombatController.combatAudioSource);

            replacement.gameObject.SetActive(true);

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().rotation = initialRotation;

            // Get y navmesh

            transform.position = Utils.GetNearestNavMeshPoint(transform.position);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            transform.rotation = initialRotation;

            GetComponent<MeshRenderer>().enabled = false;

            foreach (var colliderToDisable in collidersToDisableOnDestroy)
            {
                colliderToDisable.enabled = false;
            }
        }
    }

}
