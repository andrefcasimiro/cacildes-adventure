using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyProjectileController : MonoBehaviour
    {
        public readonly int hashShooting = Animator.StringToHash("Shooting");
        public readonly int hashIsShooting = Animator.StringToHash("IsShooting");


        [Header("Projectile Options")]
        public GameObject projectilePrefab;
        public Transform projectileSpawnPointRef;

        [Header("Shooting Settings")]
        public float rotationDurationAfterFiringProjectile = 1f;

        [Header("Trigger Settings")]
        [Range(0, 100)] public int weight = 50;
        public float minimumDistanceToFire = 4f;
        public float maxProjectileCooldown = 10f;
        private float projectileCooldown = 0f;

        private Enemy enemy => GetComponent<Enemy>();
        private EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();

        public GameObject bowGraphic;

        [HideInInspector]
        public bool isReadyToShoot = false;

        private void Start()
        {
            HideBow();
        }

        private void Update()
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

        public void PrepareProjectile()
        {
            enemy.agent.isStopped = true;
            enemy.animator.Play(hashShooting);
            projectileCooldown = 0f;
        }

        public bool CanShoot()
        {
            return projectilePrefab != null && projectileCooldown >= maxProjectileCooldown;
        }

        public bool IsShooting()
        {
            return enemy.animator.GetBool(hashIsShooting);
        }


        #region Animation Events
        public void FireProjectile()
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPointRef.transform.position, Quaternion.identity);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            projectile.Shoot(enemy.playerCombatController.headRef.transform);
        }
        #endregion

        public void ShowBow()
        {
            if (bowGraphic == null) { return; }

            if (enemyCombatController.leftHandWeapon != null)
            {
                enemyCombatController.leftHandWeapon.gameObject.SetActive(false);
            }

            if (enemyCombatController.rightHandWeapon != null)
            {
                enemyCombatController.rightHandWeapon.gameObject.SetActive(false);
            }

            bowGraphic.gameObject.SetActive(true);
        }
        public void HideBow()
        {
            if (bowGraphic == null) { return; }

            bowGraphic.gameObject.SetActive(false);

            if (enemyCombatController.leftHandWeapon != null)
            {
                enemyCombatController.leftHandWeapon.gameObject.SetActive(true);
            }

            if (enemyCombatController.rightHandWeapon != null)
            {
                enemyCombatController.rightHandWeapon.gameObject.SetActive(true);
            }

        }


    }

}