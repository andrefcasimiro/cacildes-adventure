using AF.Animations;
using AF.Equipment;
using AF.Footsteps;
using AF.Health;
using AF.Inventory;
using AF.Ladders;
using AF.Shooting;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerManager : CharacterBaseManager
    {
        public ThirdPersonController thirdPersonController;
        public PlayerWeaponsManager playerWeaponsManager;
        public ClimbController climbController;
        public DodgeController dodgeController;
        public StatsBonusController statsBonusController;
        public PlayerLevelManager playerLevelManager;
        public PlayerAchievementsManager playerAchievementsManager;
        public CombatNotificationsController combatNotificationsController;
        public PlayerCombatController playerCombatController;
        public StaminaStatManager staminaStatManager;
        public ManaManager manaManager;
        public DefenseStatManager defenseStatManager;
        public AttackStatManager attackStatManager;
        public PlayerInventory playerInventory;
        public FavoriteItemsManager favoriteItemsManager;
        public PlayerShooter playerShootingManager;
        public ProjectileSpawner projectileSpawner;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public FootstepListener footstepListener;
        public PlayerComponentManager playerComponentManager;
        public EventNavigator eventNavigator;
        public PlayerBlockInput playerBlockInput;
        public StarterAssetsInputs starterAssetsInputs;
        public PlayerAnimationEventListener playerAnimationEventListener;
        public PlayerBackstabController playerBackstabController;
        public TwoHandingController twoHandingController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        public EquipmentDatabase equipmentDatabase;

        // Animator Overrides
        protected AnimatorOverrideController animatorOverrideController;
        RuntimeAnimatorController defaultAnimatorController;

        private void Awake()
        {
            SetupAnimRefs();
        }

        void SetupAnimRefs()
        {
            if (defaultAnimatorController == null)
            {
                defaultAnimatorController = animator.runtimeAnimatorController;
            }
            if (animatorOverrideController == null)
            {
                animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            }

        }


        public override void ResetStates()
        {
            // First, reset all flags before calling the handlers
            isBusy = false;
            animator.applyRootMotion = false;

            thirdPersonController.canRotateCharacter = true;

            if (playerInventory.currentConsumedItem != null)
            {
                playerInventory.FinishItemConsumption();
            }

            playerCombatController.ResetStates();
            playerShootingManager.ResetStates();



            dodgeController.ResetStates();
            playerInventory.ResetStates();
            characterPosture.ResetStates();
            damageReceiver.ResetStates();

            playerComponentManager.ResetStates();

            playerWeaponsManager.ResetStates();
            playerWeaponsManager.ShowEquipment();

            playerBlockInput.CheckQueuedInput();
        }

        public override Damage GetAttackDamage()
        {
            return attackStatManager.GetAttackDamage();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!dodgeController.isDodging)
            {
                return;
            }

            if (other.TryGetComponent<DamageReceiver>(out var damageReceiver) && damageReceiver.damageOnDodge)
            {
                damageReceiver.TakeDamage(new Damage(
                    physical: 1,
                    fire: 0,
                    frost: 0,
                    lightning: 0,
                    darkness: 0,
                    magic: 0,
                    poiseDamage: 0,
                    postureDamage: 0,
                    weaponAttackType: WeaponAttackType.Blunt,
                    statusEffects: null,
                    pushForce: 0
                ));
            }
        }


        public void UpdateAnimatorOverrideControllerClips()
        {
            SetupAnimRefs();

            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            animator.runtimeAnimatorController = defaultAnimatorController;

            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                if (currentWeapon.animationOverrides.Count > 0)
                {
                    UpdateAnimationOverrides(animator, clipOverrides, currentWeapon.animationOverrides);
                }

                if (twoHandingController.isTwoHanding && currentWeapon.twoHandOverrides != null && currentWeapon.twoHandOverrides.Count > 0)
                {
                    UpdateAnimationOverrides(animator, clipOverrides, currentWeapon.twoHandOverrides);
                }
            }
        }

        void UpdateAnimationOverrides(Animator animator, AnimationClipOverrides clipOverrides, System.Collections.Generic.List<AnimationOverride> clips)
        {
            foreach (var animationOverride in clips)
            {
                clipOverrides[animationOverride.animationName] = animationOverride.animationClip;
                animatorOverrideController.ApplyOverrides(clipOverrides);
            }

            animator.runtimeAnimatorController = animatorOverrideController;
        }


        public void UpdateAnimatorOverrideControllerClip(string animationName, AnimationClip animationClip)
        {
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            animator.runtimeAnimatorController = defaultAnimatorController;

            clipOverrides[animationName] = animationClip;

            animatorOverrideController.ApplyOverrides(clipOverrides);
            animator.runtimeAnimatorController = animatorOverrideController;
        }
    }
}
