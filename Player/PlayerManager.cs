using AF.Animations;
using AF.Equipment;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerManager : MonoBehaviour, ICharacter
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

        public bool isBusy = false;

        public void ResetStates()
        {
            if (playerInventory.currentConsumedItem != null)
            {
                playerInventory.FinishItemConsumption();
            }

            animator.applyRootMotion = false;
            dodgeController.hasIframes = false;
            thirdPersonController.canRotateCharacter = true;
            playerComponentManager.EnableCollisionWithEnemies();
            isBusy = false;
        }

        public bool IsBusy()
        {
            return isBusy;
        }

        public void PlayBusyAnimation(int animationHash)
        {
            animator.Play(animationHash);
            isBusy = true;
        }
        public void PlayBusyAnimationWithRootMotion(int animationHash)
        {
            animator.applyRootMotion = true;
            PlayBusyAnimation(animationHash);
        }
    }
}
