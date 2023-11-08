using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class MovingPlatform : MonoBehaviour
    {
        CharacterController characterController;
        [HideInInspector] public Animator animator => GetComponent<Animator>();

        Transform originalParent;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (characterController == null)
                {
                    characterController = other.GetComponent<CharacterController>();
                }

                if (Physics.autoSyncTransforms == false)
                {
                    originalParent = characterController.transform.parent;
                    characterController.transform.SetParent(this.transform);
                    Physics.autoSyncTransforms = true;
                }

            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (characterController == null)
                {
                    characterController = other.GetComponent<CharacterController>();
                }

                characterController.transform.SetParent(originalParent);
                Physics.autoSyncTransforms = false;
            }
        }
    }

}
