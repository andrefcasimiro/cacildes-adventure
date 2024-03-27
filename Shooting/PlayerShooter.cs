using System.Collections;
using AF.Inventory;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace AF.Shooting
{
    public class PlayerShooter : CharacterBaseShooter
    {
        public readonly int hashFireBowLockedOn = Animator.StringToHash("Locked On - Shoot Bow");

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

        public float bowAimCameraDistance = 1.25f;
        public float spellAimCameraDistance = 2.25f;

        [Header("Components")]
        public LockOnManager lockOnManager;
        public UIManager uIManager;
        public MenuManager menuManager;

        [Header("Refs")]
        public Transform playerFeetRef;


        [Header("Flags")]
        public bool isAiming = false;
        public bool isShooting = false;

        // For cache purposes
        Spell previousSpell;

        [Header("Events")]
        public UnityEvent onSpellAim_Begin;
        public UnityEvent onBowAim_Begin;

        [Header("Cinemachine")]
        Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;

        public CinemachineImpulseSource cinemachineImpulseSource;

        Coroutine FireDelayedProjectileCoroutine;

        public void ResetStates()
        {
            isShooting = false;
        }

        void SetupCinemachine3rdPersonFollowReference()
        {
            if (cinemachine3RdPersonFollow != null)
            {
                return;
            }

            cinemachine3RdPersonFollow = aimingCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnFireInput()
        {
            if (CanShoot())
            {

                if (equipmentDatabase.IsBowEquipped() && equipmentDatabase.HasEnoughCurrentArrows())
                {
                    ShootBow(equipmentDatabase.GetCurrentArrow(), transform, lockOnManager.nearestLockOnTarget?.transform);
                    return;
                }

                PlayerManager playerManager = GetPlayerManager();

                if (
                   equipmentDatabase.IsStaffEquipped()
                   && equipmentDatabase.GetCurrentSpell() != null
                   && playerManager.manaManager.HasEnoughManaForSpell(equipmentDatabase.GetCurrentSpell()))
                {
                    playerManager.manaManager.DecreaseMana(equipmentDatabase.GetCurrentSpell().costPerCast);

                    HandleSpellCastAnimationOverrides();

                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashCast);
                }
            }
        }

        void HandleSpellCastAnimationOverrides()
        {
            Spell currentSpell = equipmentDatabase.GetCurrentSpell();

            if (currentSpell == previousSpell)
            {
                return;
            }

            previousSpell = currentSpell;

            if (currentSpell.castAnimationOverride != null)
            {
                GetPlayerManager().UpdateAnimatorOverrideControllerClip("Cacildes - Spell - Cast", currentSpell.castAnimationOverride);
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
            lockOnManager.DisableLockOn();

            SetupCinemachine3rdPersonFollowReference();

            if (equipmentDatabase.IsBowEquipped())
            {
                GetPlayerManager().animator.SetBool(hashIsAiming, true);

                cinemachine3RdPersonFollow.CameraDistance = bowAimCameraDistance;

                onBowAim_Begin?.Invoke();
            }
            else if (equipmentDatabase.IsStaffEquipped())
            {
                cinemachine3RdPersonFollow.CameraDistance = spellAimCameraDistance;
                onSpellAim_Begin?.Invoke();
            }

            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(false);
        }

        public void Aim_End()
        {
            if (!isAiming)
            {
                return;
            }

            isAiming = false;
            aimingCamera.SetActive(false);
            lookAtConstraint.constraintActive = false;
            GetPlayerManager().thirdPersonController.rotateWithCamera = false;
            GetPlayerManager().animator.SetBool(hashIsAiming, false);
            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (isAiming && equipmentDatabase.IsBowEquipped())
            {
                lookAtConstraint.constraintActive = GetPlayerManager().thirdPersonController._input.move.magnitude <= 0;
            }
        }
        public void ShootBow(ConsumableProjectile consumableProjectile, Transform origin, Transform lockOnTarget)
        {
            if (equipmentDatabase.IsBowEquipped())
            {
                achievementOnShootingBowForFirstTime.AwardAchievement();
            }

            inventoryDatabase.RemoveItem(consumableProjectile, 1);

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            FireProjectile(consumableProjectile.projectile.gameObject, 1f, lockOnTarget, null);
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public override void CastSpell()
        {
            ShootSpell(equipmentDatabase.GetCurrentSpell(), transform, lockOnManager.nearestLockOnTarget?.transform);
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

            if (spell.projectile != null)
            {
                FireProjectile(spell.projectile.gameObject, 0f, lockOnTarget, spell);
            }
        }

        public void FireProjectile(GameObject projectile, float originDistanceFromCamera, Transform lockOnTarget, Spell spell)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
            Vector3 lookPosition = ray.direction;

            if (lookPosition.y > 0)
            {
                lookPosition.y *= -1f;
            }

            float delay = 0f;

            if (lockOnTarget != null && lockOnManager.isLockedOn)
            {
                var rotation = lockOnTarget.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            if (equipmentDatabase.IsBowEquipped())
            {
                if (isAiming)
                {
                    characterBaseManager.PlayBusyHashedAnimation(hashFireBow);
                }
                else
                {

                    characterBaseManager.PlayBusyHashedAnimation(hashFireBowLockedOn);
                    delay = 0.5f;
                }
            }

            cinemachineImpulseSource.GenerateImpulse();
            isShooting = true;

            if (FireDelayedProjectileCoroutine != null)
            {
                StopCoroutine(FireDelayedProjectileCoroutine);
            }

            FireDelayedProjectileCoroutine = StartCoroutine(FireDelayedProjectile_Coroutine(projectile, originDistanceFromCamera, Quaternion.LookRotation(lookPosition), ray, delay, spell));
        }

        IEnumerator FireDelayedProjectile_Coroutine(GameObject projectile, float originDistanceFromCamera, Quaternion lookPosition, Ray ray, float delay, Spell spell)
        {
            yield return new WaitForSeconds(delay);

            Vector3 origin = ray.GetPoint(originDistanceFromCamera);

            // If shooting spell but not locked on, use player transform forward to direct the spell
            if (lockOnManager.isLockedOn == false && isAiming == false)
            {
                origin = lookAtConstraint.transform.position;
                ray.direction = characterBaseManager.transform.forward;

                Vector3 lookDir = ray.direction;
                lookDir.y = 0;
                lookPosition = Quaternion.LookRotation(lookDir);
            }

            if (spell != null)
            {
                if (spell.spawnAtPlayerFeet)
                {
                    origin = playerFeetRef.transform.position;
                }
            }

            GameObject projectileInstance = Instantiate(projectile.gameObject, origin, lookPosition);
            if (spell != null)
            {
                if (projectileInstance.TryGetComponent<OnDamageCollisionAbstractManager>(out var spellParticleCollisionManager))
                {
                    spellParticleCollisionManager.damageOwner = GetPlayerManager();
                }
            }

            projectileInstance.TryGetComponent(out IProjectile componentProjectile);
            if (componentProjectile == null)
            {
                yield break;
            }

            componentProjectile.Shoot(characterBaseManager, ray.direction * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());
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

            if (menuManager.isMenuOpen)
            {
                return false;
            }

            if (uIManager.IsShowingGUI())
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

            // If not ranged weapons equipped, dont allow shooting
            if (
                !equipmentDatabase.IsBowEquipped()
                && !equipmentDatabase.IsStaffEquipped())
            {
                return false;
            }

            return true;
        }

        PlayerManager GetPlayerManager()
        {
            return characterBaseManager as PlayerManager;
        }

    }

}
