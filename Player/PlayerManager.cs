using AF.Animations;
using AF.Equipment;
using AF.Health;
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
        public DefenseStatManager defenseStatManager;
        public AttackStatManager attackStatManager;
        public PlayerInventory playerInventory;
        public FavoriteItemsManager favoriteItemsManager;
        public PlayerSpellManager playerSpellManager;
        public PlayerShooter playerShootingManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public FootstepListener footstepListener;
        public PlayerComponentManager playerComponentManager;
        public PlayerPoiseController playerPoiseController;
        public EventNavigator eventNavigator;
        public PlayerBlockInput playerBlockInput;
        public StarterAssetsInputs starterAssetsInputs;
        public PlayerAnimationEventListener playerAnimationEventListener;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        public override void ResetStates()
        {
            // First, reset all flags before calling the handlers
            isBusy = false;
            animator.applyRootMotion = false;
            dodgeController.hasIframes = false;
            thirdPersonController.canRotateCharacter = true;

            if (playerInventory.currentConsumedItem != null)
            {
                playerInventory.FinishItemConsumption();
            }

            characterPosture.ResetStates();

            playerComponentManager.EnableCollisionWithEnemies();
            playerWeaponsManager.ShowEquipment();

            playerBlockInput.CheckQueuedInput();
        }

        public override Damage GetAttackDamage()
        {
            return attackStatManager.GetAttackDamage();
        }
    }
}
