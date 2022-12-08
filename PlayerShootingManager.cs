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

            if (Player.instance.ownedItems.FirstOrDefault(x => x.item == bow) == null)
            {
                notificationManager.ShowNotification("Bow required to use this item", notificationManager.systemError);
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

        public bool IsShooting()
        {
            return animator.GetBool("IsShooting");
        }

        bool CanShoot()
        {
            if (playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification("Can't shoot while attacking", notificationManager.systemError);
                return false;
            }

            if (playerParryManager.IsBlocking() || playerParryManager.IsParrying())
            {
                notificationManager.ShowNotification("Can't shoot while blocking", notificationManager.systemError);
                return false;
            }

            if (dodgeController.IsDodging())
            {
                notificationManager.ShowNotification("Can't shoot while dodging", notificationManager.systemError);
                return false;
            }
            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                notificationManager.ShowNotification("Can't shoot while climbing", notificationManager.systemError);
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
        #endregion

    }

}
