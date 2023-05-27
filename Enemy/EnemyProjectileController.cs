using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyProjectileController : MonoBehaviour
    {

        [Header("Shooting Options")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPointRef;
        public float rotationDurationAfterFiringProjectile = 1f;
        [Range(0, 100)] public int shootingWeight = 50;
        public float minimumDistanceToFire = 4f;
        public float maximumDistanceToFire = 999f;
        public float maxProjectileCooldown = 10f;
        public GameObject bowGraphic;
        public bool isReadyToShoot = false;

        private float projectileCooldown = 0f;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        ClimbController playerClimbController;

        [Header("Animations")]
        public string[] shootActions;

        private void Awake()
        {
            playerClimbController = FindObjectOfType<ClimbController>(true);
        }

        void Start()
        {
            HideBow();
        }

        private void Update()
        {
            UpdateProjectile();
        }

        void UpdateProjectile()
        {
            if (projectilePrefab == null)
            {
                return;
            }

            if (projectileCooldown <= maxProjectileCooldown)
            {
                projectileCooldown += Time.deltaTime;
            }

            if (IsShooting() == false)
            {
                projectileCooldown += Time.deltaTime;
            }
        }

        public void ShowBow()
        {
            if (bowGraphic == null)
            {
                return;
            }

            enemyManager.enemyWeaponController.HideWeapons();

            bowGraphic.SetActive(true);
        }

        public void HideBow()
        {
            if (bowGraphic == null)
            {
                return;
            }

            enemyManager.enemyWeaponController.ShowWeapons();

            bowGraphic.SetActive(false);
        }

        public void PrepareProjectile()
        {
            enemyManager.animator.Play(this.shootActions.Length > 0 ? Animator.StringToHash(this.shootActions[0]) : enemyManager.hashShooting);
            projectileCooldown = 0f;
        }

        public bool CanShoot()
        {
            return projectilePrefab != null && projectileCooldown >= maxProjectileCooldown && Vector3.Distance(transform.position, playerClimbController.transform.position) <= maximumDistanceToFire;
        }

        public bool IsShooting()
        {
            return enemyManager.animator.GetBool(enemyManager.hashIsShooting);
        }

        /// <summary>
        ///  Animation Event
        /// </summary>
        public void FireProjectile()
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPointRef.transform.position, Quaternion.identity);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();

            if (projectile.forceTrajectoryTowardsPlayer)
            {
                float dist = Vector3.Distance(playerClimbController.transform.position, transform.position);
                projectile.forwardVelocity *= dist > 1 ? dist : 1f;
            }

            projectile.Shoot(playerClimbController.playerHeadRef.transform);
        }

    }

}
