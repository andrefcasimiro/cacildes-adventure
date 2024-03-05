using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AF.Ladders;
using AF.StatusEffects;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerInventory : MonoBehaviour
    {
        public Consumable currentConsumedItem;
        GameObject consumableGraphicInstance;

        public Transform handItemRef;

        [Header("UI Components")]
        public NotificationManager notificationManager;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

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

        [Header("Events")]
        public UnityEvent onResetState;

        // Consts
        private const string CANT_CONSUME_ITEM_AT_THIS_TIME = "Can't consume item at this time";

        public void ResetStates()
        {
            isConsumingItem = false;
            onResetState?.Invoke();
        }

        public void ReplenishItems()
        {
            inventoryDatabase.ReplenishItems();

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        void HandleItemAchievements(Item item)
        {
            if (item is Weapon)
            {
                int numberOfWeapons = inventoryDatabase.GetWeaponsCount();

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
                int numberOfSpells = inventoryDatabase.GetSpellsCount();

                if (numberOfSpells <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstSpell.AwardAchievement();
                }
            }
        }

        public void AddItem(Item item, int quantity)
        {
            HandleItemAchievements(item);

            inventoryDatabase.AddItem(item, quantity);

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

            if (consumable.isRenewable && inventoryDatabase.GetItemAmount(consumable) <= 0)
            {
                notificationManager.ShowNotification("Consumable depleted", notificationManager.notEnoughSpells);
                return;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(CANT_CONSUME_ITEM_AT_THIS_TIME, notificationManager.systemError);
                return;
            }

            if (playerManager.characterPosture.isStunned)
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


            foreach (StatusEffect statusEffect in currentConsumedItem.statusEffectsWhenConsumed)
            {
                playerManager.statusController.statusEffectInstances.FirstOrDefault(x => x.Key == statusEffect).Value?.onConsumeStart?.Invoke();
            }

            if (consumable.shouldHideEquipmentWhenConsuming)
            {
                playerManager.playerWeaponsManager.HideEquipment();
            }

            if (consumable.isBossToken)
            {
                uIDocumentPlayerGold.AddGold((int)consumable.value);
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
