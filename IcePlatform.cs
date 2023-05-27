using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class IcePlatform : MonoBehaviour
    {
        CharacterController characterController;

        public float velocity = 5f;

        Vector3 transformForward;

        ThirdPersonController tps;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (tps == null)
                {
                    tps = other.GetComponent<ThirdPersonController>();
                }

                tps.isSliding = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                transformForward = Vector3.zero;
                tps.isSliding = false;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (characterController == null)
                {
                    characterController = other.GetComponent<CharacterController>();
                }

                transformForward = other.transform.forward;
            }
        }

        private void LateUpdate()
        {
            if (tps != null && !tps.isSliding)
            {
                return;
            }

            if (transformForward != Vector3.zero && characterController != null)
            {
                characterController.Move(transformForward * velocity * Time.deltaTime);
            }

        }
    }

}
