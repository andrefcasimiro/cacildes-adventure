using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyPushableOnDeath : MonoBehaviour
    {
        public bool isActivated = false;

        public float pushForce = 50f;
        public int damageOnCollidingWithEnemies = 150;
        public float damageActiveTime = 1f;

        public bool canDamage = false;

        public AudioClip pushSfx;

        CharacterManager characterManager => GetComponent<CharacterManager>();

        public void Activate()
        {
            isActivated = true;

            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                rigidbody.useGravity = true;

            }


        }
        public void Deactivate()
        {
            isActivated = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActivated == false || canDamage == false)
            {
                return;
            }

            // EnemyHealthController en = other.GetComponent<EnemyHealthController>();
            /*Pif (en == null)
            {
                en = other.GetComponentInParent<EnemyHealthController>();
            }

            if (en != null)
            {
                en.TakeEnvironmentalDamage(damageOnCollidingWithEnemies);
                canDamage = false;
            }*/
        }

        public void Throw()
        {
            // characterManager.PushEnemy(pushForce, ForceMode.Acceleration);

            canDamage = true;

            StartCoroutine(ResetCanDamage());

            characterManager.combatAudioSource.PlayOneShot(pushSfx);
        }

        IEnumerator ResetCanDamage()
        {
            yield return new WaitForSeconds(damageActiveTime);
            canDamage = false;
        }
    }

}
