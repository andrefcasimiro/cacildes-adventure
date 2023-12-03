using System.Collections;
using AF.Inventory;
using UnityEngine;
using UnityEngine.Animations;

namespace AF.Shooting
{
    public class PlayerShooter : CharacterBaseShooter
    {

        [Header("Stamina Cost")]
        public int minimumStaminaToShoot = 10;

        [Header("Achievements")]
        public Achievement achievementOnShootingBowForFirstTime;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Aiming")]
        public GameObject aimingCamera;

        public LookAtConstraint lookAtConstraint;

        [Header("Flags")]
        public bool isAiming = false;

        public Transform fireTransform;

        public StarterAssetsInputs starterAssetsInputs;

        public void OnFireInput()
        {
            if (CanShoot() && isAiming)
            {
                if (equipmentDatabase.IsBowEquipped())
                {
                    ShootBow(equipmentDatabase.GetCurrentArrow(), transform, null);
                }
                else if (equipmentDatabase.IsStaffEquipped())
                {
                    GetPlayerManager().PlayBusyHashedAnimationWithRootMotion(hashCast);
                }
            }
        }

        public void Aim_Begin()
        {
            if (!CanAim())
            {
                return;
            }

            isAiming = true;
            aimingCamera.SetActive(true);
            GetPlayerManager().thirdPersonController.rotateWithCamera = true;

            if (equipmentDatabase.IsBowEquipped())
            {
                GetPlayerManager().animator.SetBool(hashIsAiming, true);
            }

            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isAiming && equipmentDatabase.IsBowEquipped())
            {
                lookAtConstraint.constraintActive = GetPlayerManager().thirdPersonController._input.move.magnitude <= 0;
            }
        }

        public void Aim_End()
        {
            isAiming = false;
            aimingCamera.SetActive(false);
            lookAtConstraint.constraintActive = false;
            GetPlayerManager().thirdPersonController.rotateWithCamera = false;
            GetPlayerManager().animator.SetBool(hashIsAiming, false);
            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(true);
        }

        PlayerManager GetPlayerManager()
        {
            return characterBaseManager as PlayerManager;
        }

        public void ShootBow(ConsumableProjectile consumableProjectile, Transform origin, Transform lockOnTarget)
        {
            if (equipmentDatabase.IsBowEquipped())
            {
                achievementOnShootingBowForFirstTime.AwardAchievement();
            }

            inventoryDatabase.RemoveItem(consumableProjectile, 1);

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            FireProjectile(consumableProjectile.projectile.gameObject, 1f, lockOnTarget);
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public override void CastSpell()
        {
            ShootSpell(equipmentDatabase.GetCurrentSpell(), transform, null);
        }

        public override void FireArrow()
        {
        }

        public void ShootSpell(Spell spell, Transform origin, Transform lockOnTarget)
        {
            if (spell == null)
            {
                return;
            }

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            FireProjectile(spell.spellCastParticle.gameObject, 10f, lockOnTarget);
        }

        void FireProjectile(GameObject projectile, float originDistanceFromCamera, Transform lockOnTarget)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
            Vector3 lookPosition = ray.direction;

            GameObject projectileInstance = Instantiate(projectile.gameObject, ray.GetPoint(originDistanceFromCamera), Quaternion.LookRotation(lookPosition));

            projectileInstance.TryGetComponent(out IProjectile componentProjectile);
            if (componentProjectile == null)
            {
                return;
            }

            if (lookPosition.y > 0)
            {
                lookPosition.y *= -1f;
            }

            componentProjectile.Shoot(null, ray.direction * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());

            if (equipmentDatabase.IsBowEquipped())
            {
                characterBaseManager.PlayBusyHashedAnimation(hashFireBow);
            }

            if (lockOnTarget != null)
            {
                var rotation = lockOnTarget.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }
        }

        bool CanAim()
        {
            if (GetPlayerManager().IsBusy())
            {
                return false;
            }

            return equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped();
        }

        public override bool CanShoot()
        {
            if (playerStatsDatabase.currentStamina < minimumStaminaToShoot)
            {
                return false;
            }

            if (GetPlayerManager().IsBusy())
            {
                return false;
            }

            if (GetPlayerManager().characterBlockController.isBlocking)
            {
                return false;
            }

            /*
            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantShootArrowsAtThisTime(), notificationManager.systemError);
                return false;
            }*/

            return equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped();
        }
    }

}
