using System.Linq;
using AF.Inventory;
using AF.Ladders;
using AF.StatusEffects;
using UnityEngine;

namespace AF
{
    public class PlayerInventory : MonoBehaviour
    {
        public readonly int hashDrinking = Animator.StringToHash("Drinking");
        public readonly int hashEating = Animator.StringToHash("Eating");
        public readonly int hashClock = Animator.StringToHash("Clock");
        public readonly int hashIsConsumingItem = Animator.StringToHash("IsConsumingItem");

        public Consumable currentConsumedItem;
        GameObject consumableGraphicInstance;

        public Transform handItemRef;

        [Header("UI Components")]
        public NotificationManager notificationManager;
        public ViewClockMenu viewClockMenu;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;

        [SerializeField] private UIManager uIManager;
        [SerializeField] private MenuManager menuManager;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;
        public StatusDatabase statusDatabase;

        [Header("Flags")]
        public bool isConsumingItem = false;

        // Consts
        private const string CANT_CONSUME_ITEM_AT_THIS_TIME = "Can't consume item at this time";

        public void ResetStates()
        {
            isConsumingItem = false;
        }

        public void ReplenishItems()
        {
            foreach (var item in inventoryDatabase.ownedItems)
            {
                if (item.Value.usages > 0)
                {
                    item.Value.amount += item.Value.usages;
                    item.Value.usages = 0;
                }
            }

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        void HandleItemAchievements(Item item)
        {
            if (item is Weapon)
            {
                int numberOfWeapons = inventoryDatabase.ownedItems.Count(x => x.Key is Weapon);

                if (numberOfWeapons <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstWeapon.AwardAchievement();
                }
                else if (numberOfWeapons == 10)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringTenWeapons.AwardAchievement();
                }
            }
            else if (item is Spell)
            {
                int numberOfSpells = inventoryDatabase.ownedItems.Count(x => x.Key is Spell);

                if (numberOfSpells <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstSpell.AwardAchievement();
                }
            }
        }

        public void AddItem(Item item, int quantity)
        {
            HandleItemAchievements(item);

            if (inventoryDatabase.HasItem(item))
            {
                inventoryDatabase.ownedItems[item].amount += quantity;
            }
            else
            {
                inventoryDatabase.ownedItems.Add(item, new ItemAmount() { amount = quantity, usages = 0 });
            }


            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        public void RemoveItem(Item item, int quantity)
        {
            inventoryDatabase.RemoveItem(item, quantity);

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        public void PrepareItemForConsuming(Consumable consumable)
        {
            if (isConsumingItem)
            {
                return;
            }

            if (consumable.lostUponUse == false && inventoryDatabase.GetItemAmount(consumable) <= 0)
            {
                notificationManager.ShowNotification("Consumable depleted", notificationManager.notEnoughSpells);
                return;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(CANT_CONSUME_ITEM_AT_THIS_TIME, notificationManager.systemError);
                return;
            }

            if (playerManager.characterPosture.IsStunned())
            {
                notificationManager.ShowNotification(CANT_CONSUME_ITEM_AT_THIS_TIME, notificationManager.systemError);
                return;
            }

            if (playerManager.dodgeController.isDodging)
            {
                notificationManager.ShowNotification(CANT_CONSUME_ITEM_AT_THIS_TIME, notificationManager.systemError);
                return;
            }

            if (!playerManager.thirdPersonController.Grounded)
            {
                notificationManager.ShowNotification(CANT_CONSUME_ITEM_AT_THIS_TIME, notificationManager.systemError);
                return;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                notificationManager.ShowNotification(CANT_CONSUME_ITEM_AT_THIS_TIME, notificationManager.systemError);
                return;
            }

            if (playerManager.isBusy)
            {
                return;
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                return;
            }

            this.currentConsumedItem = consumable;
            isConsumingItem = true;

            if (consumable.isAlcoholic)
            {
                playerManager.playerAchievementsManager.achievementForDrinkingFirstAlcoholicBeverage.AwardAchievement();
            }

            if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.DRINK)
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashDrinking);
            }
            else if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.EAT)
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashEating);
            }
            else if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.CLOCK)
            {

                if (menuManager.isMenuOpen)
                {
                    menuManager.CloseMenu();
                }

                if (uIManager.CanShowGUI() == false)
                {
                    uIManager.ShowCanNotAccessGUIAtThisTime();
                    return;
                }


                // Find Clock
                viewClockMenu.gameObject.SetActive(true);
                return;
            }

            playerManager.playerWeaponsManager.HideEquipment();

            if (consumable.graphic != null)
            {
                consumableGraphicInstance = Instantiate(consumable.graphic, handItemRef);
            }

            playerManager.playerComponentManager.DisableCharacterController();
            playerManager.playerComponentManager.DisableComponents();
        }

        public void FinishItemConsumption()
        {
            if (currentConsumedItem == null)
            {
                return;
            }

            playerManager.playerComponentManager.EnableCharacterController();
            playerManager.playerComponentManager.EnableComponents();

            if (currentConsumedItem.shouldNotRemoveOnUse == false)
            {
                playerManager.playerInventory.RemoveItem(currentConsumedItem, 1);
            }

            if (currentConsumedItem.statusesToRemove.Length > 0)
            {
                foreach (StatusEffect statusEffectToRemove in currentConsumedItem.statusesToRemove)
                {
                    AppliedStatusEffect appliedStatusEffect = playerManager.statusController.appliedStatusEffects.FirstOrDefault(
                        x => x.statusEffect == statusEffectToRemove);

                    if (appliedStatusEffect != null)
                    {
                        playerManager.statusController.RemoveAppliedStatus(appliedStatusEffect);
                    }
                }
            }

            foreach (StatusEffect statusEffect in currentConsumedItem.statusEffectsWhenConsumed)
            {
                // For positive effects, we override the status effect resistance to be the duration of the consumable effect
                playerManager.statusController.statusEffectResistances[statusEffect] = currentConsumedItem.effectsDurationInSeconds;

                playerManager.statusController.InflictStatusEffect(statusEffect, currentConsumedItem.effectsDurationInSeconds, true);
            }

            currentConsumedItem = null;
            Destroy(this.consumableGraphicInstance);
        }
    }
}
