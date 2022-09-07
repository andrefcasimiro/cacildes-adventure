using UnityEngine;

namespace AF
{

    public class Projectile : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float maxRange = 10f;

        public float projectileDamage = 50f;

        [Header("Sound")]
        public AudioClip projectileImpactSfx;

        Transform playerPosition;
        Transform originalPosition;
        Rigidbody rigidBody => GetComponent<Rigidbody>();

        private void Start()
        {
            originalPosition = transform;
        }

        public void Shoot(Transform playerPosition)
        {
            this.playerPosition = playerPosition;
            transform.rotation = Quaternion.LookRotation(playerPosition.transform.position - transform.position);
        }

        private void Update()
        {
            if (playerPosition == null)
            {
                return;
            }

            rigidBody.AddForce(transform.forward * moveSpeed, ForceMode.Impulse);

            bool hasReachedDestination = Vector3.Distance(transform.position, playerPosition.position) <= 0.1f;
            bool hasReachedLimit = Vector3.Distance(originalPosition.position, transform.position) > maxRange;

            if (hasReachedDestination || hasReachedLimit)
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

            Player player = other.gameObject.GetComponent<Player>();

            // Check if player is facing enemy, other wise ignore shield
            bool targetFacingProjectile = Vector3.Angle(transform.forward * -1, playerPosition.transform.forward) <= 90f;
            if (player.isBlocking && targetFacingProjectile)
            {
                Utils.FaceTarget(transform, player.transform);
                return;
            }

            if (player.IsBusy() || player.isDodging)
            {
                return;
            }

            if (projectileImpactSfx != null)
            {
                GetComponent<AudioSource>().PlayOneShot(projectileImpactSfx);
            }

            player.healthbox.TakeDamage(projectileDamage, this.transform, null);
        }
    }

}