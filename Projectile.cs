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
        public int projectilePoiseDamage = 5;

        [Header("Sound")]
        public AudioClip projectileImpactOnBodySfx;
        public AudioClip projectileImpactOnGroundSfx;

        [Header("FX")]
        public GameObject particleFx;

        Rigidbody rigidBody => GetComponent<Rigidbody>();
        Transform currentTarget;
        Vector3 targetDestination;

        [HideInInspector]
        public bool isFromPlayer = false;

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
        public void Shoot(Vector3 position, bool directToEnemy)
        {
            targetDestination = position;

            transform.parent = null;

            Quaternion arrowRotation = Quaternion.LookRotation(targetDestination - transform.position);
            transform.rotation = arrowRotation;

            if (directToEnemy == false)
            {
                rigidBody.AddForce(this.transform.up * upwardsVelocity, forceMode);
            }

            rigidBody.AddForce(this.transform.forward * forwardVelocity, forceMode);

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
            Debug.Log(other.gameObject.name);

            if (hasCollidedAlready == true && isFromPlayer == false)
            {
                return;
            }

            if (other.gameObject.tag != "Enemy" && isFromPlayer == false)
            {
                hasCollidedAlready = true;
            }

            particleFx.gameObject.SetActive(false);

            // Is Player ?
            if (isFromPlayer)
            {
                EnemyHealthHitbox enemyHealthHitbox = other.gameObject.GetComponentInChildren<EnemyHealthHitbox>(true);

                // Colliding with something else
                if (enemyHealthHitbox == null)
                {
                    if (projectileImpactOnBodySfx != null)
                    {
                        GetComponent<AudioSource>().PlayOneShot(projectileImpactOnBodySfx);
                    }

                    return;
                }

                enemyHealthHitbox.enemyHealthController.TakeProjectileDamage(projectileDamage, this);
                return;
            }

            PlayerHealthbox playerHealthbox = null;

            if (other.gameObject.tag == "PlayerHealthbox")
            {
                playerHealthbox = other.gameObject.GetComponent<PlayerHealthbox>();
            }
            else if (other.gameObject.tag == "Player")
            {
                playerHealthbox = other.gameObject.GetComponentInChildren<PlayerHealthbox>(true);
            }

            if (playerHealthbox != null)
            {
                var playerParrymanager = FindObjectOfType<PlayerParryManager>(true);

                if (playerParrymanager.IsBlocking() && Vector3.Angle(transform.forward, playerParrymanager.transform.forward * -1) <= 90f)
                {
                    Instantiate(Player.instance.equippedShield.blockFx, other.transform.position, Quaternion.identity);
                    return;
                }

                playerHealthbox.TakeDamage(projectileDamage, this.transform, projectileImpactOnBodySfx, projectilePoiseDamage);
            }


        }
    }

}
