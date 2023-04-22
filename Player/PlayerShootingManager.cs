using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class PlayerShootingManager : MonoBehaviour
    {
        Animator animator => GetComponent<Animator>();


        // Set by the consumed consumable
        [HideInInspector] public Projectile currentProjectile;

        LockOnManager lockOnManager;
        public Transform projectileSpawnPointRef;
        public Transform throwSpawnPointRef;

        public Item bow;

        NotificationManager notificationManager;
        PlayerInventory playerInventory;
        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        ClimbController climbController => GetComponent<ClimbController>();
        DodgeController dodgeController => GetComponent<DodgeController>();
        PlayerParryManager playerParryManager => GetComponent<PlayerParryManager>();
        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        PlayerComponentManager playerComponentManager => GetComponent<PlayerComponentManager>();

        [Header("Sounds")]
        public AudioClip bowDraw;
        public AudioClip bowReleaseArrow;

        private void Start()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);

            bow = Resources.Load<Item>("Items/Key Items/Bow");

            lockOnManager = FindObjectOfType<LockOnManager>(true);
        }

        public void ShootBow(ConsumableProjectile consumableProjectile)
        {
            if (CanShoot() == false)
            {
                return;
            }

            if (Player.instance.ownedItems.FirstOrDefault(x => x.item.name.GetEnglishText() == bow.name.GetEnglishText()) == null)
            {
                notificationManager.ShowNotification(LocalizedTerms.BowRequired(), notificationManager.systemError);
                return;
            }

            this.currentProjectile = consumableProjectile.projectile;

            playerInventory.RemoveItem(consumableProjectile, 1);

            animator.Play("Preparing Arrow");

            if (lockOnManager.nearestLockOnTarget != null)
            {
                var rotation = lockOnManager.nearestLockOnTarget.transform.position - transform.position;
                rotation.y = 0;

                playerComponentManager.DisableCharacterController();
                this.transform.rotation = Quaternion.LookRotation(rotation);
                playerComponentManager.EnableCharacterController();
            }

        }

        public void ThrowConsumable(ConsumableProjectile consumableProjectile)
        {
            if (CanShoot() == false)
            {
                return;
            }

            if (playerInventory.IsConsumingItem())
            {
                return;
            }

            this.currentProjectile = consumableProjectile.projectile;

            playerInventory.RemoveItem(consumableProjectile, 1);

            animator.Play("Throwing");

            if (currentProjectile.throwSfx != null)
            {
                playerCombatController.combatAudioSource.PlayOneShot(currentProjectile.throwSfx);
            }

            if (lockOnManager.nearestLockOnTarget != null)
            {
                var rotation = lockOnManager.nearestLockOnTarget.transform.position - transform.position;
                rotation.y = 0;

                playerComponentManager.DisableCharacterController();
                this.transform.rotation = Quaternion.LookRotation(rotation);
                playerComponentManager.EnableCharacterController();
            }

        }

        public bool IsShooting()
        {
            return animator.GetBool("IsShooting");
        }

        bool CanShoot()
        {
            if (playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantShootArrowsAtThisTime(), notificationManager.systemError);
                return false;
            }

            if (playerParryManager.IsBlocking() || playerParryManager.IsParrying())
            {
                notificationManager.ShowNotification(LocalizedTerms.CantShootArrowsAtThisTime(), notificationManager.systemError);
                return false;
            }

            if (dodgeController.IsDodging())
            {
                notificationManager.ShowNotification(LocalizedTerms.CantShootArrowsAtThisTime(), notificationManager.systemError);
                return false;
            }
            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantShootArrowsAtThisTime(), notificationManager.systemError);
                return false;
            }

            // If player is on the ground for being stunned
            if (animator.GetBool("IsStunned"))
            {
                return false;
            }

            return true;
        }

        #region Animation Events
        public void ShowBow()
        {
            if (equipmentGraphicsHandler.bow != null)
            {
                BGMManager.instance.PlaySound(bowDraw, playerCombatController.combatAudioSource);
                equipmentGraphicsHandler.bow.gameObject.SetActive(true);
            }
        }

        public void FireProjectile()
        {
            BGMManager.instance.PlaySound(bowReleaseArrow, playerCombatController.combatAudioSource);

            GameObject projectileInstance = Instantiate(currentProjectile.gameObject, projectileSpawnPointRef.position, transform.rotation);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            projectile.isFromPlayer = true;
            projectile.Shoot(lockOnManager.nearestLockOnTarget != null ? lockOnManager.nearestLockOnTarget.transform.position : this.transform.position + this.transform.forward * 10f, lockOnManager.nearestLockOnTarget != null);
        }

        public void OnThrow()
        {
            GameObject projectileInstance = Instantiate(currentProjectile.gameObject, throwSpawnPointRef.position, transform.rotation);

            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            projectile.isFromPlayer = true;

            if (lockOnManager.nearestLockOnTarget != null)
            {
                float dist = Vector3.Distance(lockOnManager.nearestLockOnTarget.transform.position, transform.position);
                projectile.forwardVelocity *= dist > 1 ? dist : 1f;
            }

            projectile.Shoot(lockOnManager.nearestLockOnTarget != null ? lockOnManager.nearestLockOnTarget.transform.position : this.transform.position + this.transform.forward * 10f, lockOnManager.nearestLockOnTarget != null);
        }
        #endregion

    }

}
