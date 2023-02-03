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
        public float maxProjectileCooldown = 10f;
        public GameObject bowGraphic;
        public bool isReadyToShoot = false;

        private float projectileCooldown = 0f;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        ClimbController playerClimbController => FindObjectOfType<ClimbController>(true);

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
            enemyManager.animator.CrossFade(enemyManager.hashShooting, 0.15f);
            projectileCooldown = 0f;
        }

        public bool CanShoot()
        {
            return projectilePrefab != null && projectileCooldown >= maxProjectileCooldown;
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
            projectile.Shoot(playerClimbController.playerHeadRef.transform);
        }

    }

}
