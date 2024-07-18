using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class SkateTrigger : MonoBehaviour
    {
        public bool isExitingArea = false;

        ThirdPersonController tps;

        CharacterController characterController;


        public float velocity = 9f;

        public int lightDamage = 15;
        public int mediumDamage = 35;
        public int largeDamage = 75;

        public UnityEvent onPlayerEnter;

        private void Awake()
        {
            tps = FindObjectOfType<ThirdPersonController>(true);
            characterController = tps.GetComponent<CharacterController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") == false)
            {
                return;
            }

            tps.isSliding = !isExitingArea;



            characterController.GetComponent<Animator>().SetBool("IsCrouched", !isExitingArea);

            onPlayerEnter.Invoke();
        }

        private void LateUpdate()
        {
            if (!IsSkating())
            {
                return;
            }

            Vector3 mov = (characterController.transform.forward) * velocity * Time.deltaTime;
            mov.y = -9.71f;

        }

        public bool IsSkating()
        {
            if (tps != null && !tps.isSliding)
            {
                return false;
            }

            return true;
        }

        public void DamageLight()
        {
            if (!IsSkating())
            {
                return;
            }

        }

        public void DamageMedium()
        {
            if (!IsSkating())
            {
                return;
            }
        }
        public void DamageLarge()
        {
            if (!IsSkating())
            {
                return;
            }
        }




    }

}
