using AF.Equipment;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerManager : MonoBehaviour
    {
        public Animator animator;
        public CharacterController characterController;
        public ThirdPersonController thirdPersonController;
        public PlayerWeaponsManager playerWeaponsManager;
        public ClimbController climbController;
        public DodgeController dodgeController;
        public StatsBonusController statsBonusController;
        public PlayerLevelManager playerLevelManager;
        public PlayerAchievementsManager playerAchievementsManager;
        public HealthStatManager healthStatManager;
        public CombatNotificationsController combatNotificationsController;
        public PlayerCombatController playerCombatController;
        public StaminaStatManager staminaStatManager;
        public DefenseStatManager defenseStatManager;
        public PlayerStatusManager playerStatusManager;
        public AttackStatManager attackStatManager;
        public PlayerInventory playerInventory;
        public FavoriteItemsManager favoriteItemsManager;
        public PlayerConsumablesManager playerConsumablesManager;
        public PlayerSpellManager playerSpellManager;
        public PlayerShootingManager playerShootingManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public FootstepListener footstepListener;
        public PlayerComponentManager playerComponentManager;
        public PlayerPoiseController playerPoiseController;
        public EventNavigator eventNavigator;
        public PlayerParryManager playerParryManager;

        public bool IsBusy = false;

        public void ResetStates()
        {
            if (playerInventory.currentConsumedItem != null)
            {
                playerInventory.FinishItemConsumption();
            }


            animator.applyRootMotion = false;
            dodgeController.hasIframes = false;
            playerComponentManager.EnableCollisionWithEnemies();
            IsBusy = false;
        }

        public void PlayBusyAnimation(int animationHash)
        {
            animator.Play(animationHash);
            IsBusy = true;
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void EnableRootMotion()
        {
            animator.applyRootMotion = true;
        }

    }
}
