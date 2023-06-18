using StarterAssets;
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

        PlayerPoiseController playerPoiseController;
        PlayerHealthbox playerHealthbox;

        public float velocity = 9f;

        public int lightDamage = 15;
        public int mediumDamage = 35;
        public int largeDamage = 75;

        public UnityEvent onPlayerEnter;

        private void Awake()
        {
            tps = FindObjectOfType<ThirdPersonController>(true);
            characterController = tps.GetComponent<CharacterController>();
            playerPoiseController = tps.GetComponent<PlayerPoiseController>();
            playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") == false)
            {
                return;
            }

            tps.isSliding = !isExitingArea;



            characterController.GetComponent<Animator>().SetBool("IsCrouched", !isExitingArea);

            tps.skateRotation = !isExitingArea;

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

            if (playerPoiseController.isStunned == false)
            {
                characterController.Move(mov);
            }
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

            playerHealthbox.Event_TakeDamage(lightDamage);
        }

        public void DamageMedium()
        {
            if (!IsSkating())
            {
                return;
            }
            playerHealthbox.Event_TakeDamage(mediumDamage);
        }
        public void DamageLarge()
        {
            if (!IsSkating())
            {
                return;
            }
            playerHealthbox.Event_TakeDamage(largeDamage);
            playerPoiseController.ActivateMaxPoiseDamage();
        }




    }

}
