using UnityEngine;

namespace AF
{

    public class Projectile : MonoBehaviour
    {
        [Header("Lifespan")]
        public float flyingDuration = 2;
        public float timeBeforeDestroying = 10;
        float lifeTimer = 0f;

        [Header("Range")]
        public float maxRange = 10f;

        [Header("Velocity")]
        public ForceMode forceMode;
        public float forwardVelocity = 10f;
        public float upwardsVelocity = 10f;

        [Header("Stats")]
        public float projectileDamage = 50f;

        [Header("Sound")]
        public AudioClip projectileImpactOnBodySfx;
        public AudioClip projectileImpactOnGroundSfx;

        [Header("FX")]
        public GameObject particleFx;

        Rigidbody rigidBody => GetComponent<Rigidbody>();
        Transform currentTarget;
        Vector3 targetDestination;

        bool hasCollidedAlready = false;

        public void Shoot(Transform target)
        {
            this.currentTarget = target;
            targetDestination = target.transform.position;

            transform.parent = null;


            Quaternion arrowRotation = Quaternion.LookRotation(targetDestination - transform.position);
            transform.rotation = arrowRotation;
            rigidBody.AddForce(this.transform.forward * forwardVelocity, forceMode);
            rigidBody.AddForce(this.transform.up * upwardsVelocity, forceMode);
            
        }

        private void Update()
        {
            if (currentTarget == null)
            {
                return;
            }

            lifeTimer += Time.deltaTime;


            if (lifeTimer >= timeBeforeDestroying)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            if (hasCollidedAlready == true)
            {
                return;
            }

            hasCollidedAlready = true;

            particleFx.gameObject.SetActive(false);

            PlayerHealthbox playerHealthbox = other.gameObject.GetComponentInChildren<PlayerHealthbox>(true);
            if (playerHealthbox == null)
            {
                // Colliding with something else

                if (projectileImpactOnBodySfx != null)
                {
                    GetComponent<AudioSource>().PlayOneShot(projectileImpactOnBodySfx);
                }
                
                return;
            }

            playerHealthbox.TakeDamage(projectileDamage, this.transform, projectileImpactOnBodySfx);


        }
    }

}
