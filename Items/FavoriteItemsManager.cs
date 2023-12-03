using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TigerForge;
using AF.Events;
using AF.Inventory;
using AF.StatusEffects;

namespace AF
{

    public class FavoriteItemsManager : MonoBehaviour
    {
        [Header("UI Components")]
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;

        [Header("Game Session")]
        public GameSession gameSession;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        [Header("Components")]
        public PlayerManager playerManager;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnConsumableUse()
        {
            if (!uIDocumentPlayerHUDV2.IsEquipmentDisplayed())
            {
                return;
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                return;
            }

            Item currentItem = equipmentDatabase.GetCurrentConsumable();

            if (currentItem == null)
            {
                return;
            }

            int itemAmount = inventoryDatabase.GetItemAmount(currentItem);

            if (itemAmount <= 1 && currentItem.lostUponUse)
            {
                equipmentDatabase.UnequipConsumable(equipmentDatabase.currentConsumableIndex);
            }

            Consumable consumableItem = currentItem as Consumable;
            playerManager.playerInventory.PrepareItemForConsuming(consumableItem);

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchWeapon()
        {
            if (!uIDocumentPlayerHUDV2.IsEquipmentDisplayed())
            {
                return;
            }

            equipmentDatabase.SwitchToNextWeapon();

            uIDocumentPlayerHUDV2.OnSwitchWeapon();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchShield()
        {
            if (!uIDocumentPlayerHUDV2.IsEquipmentDisplayed())
            {
                return;
            }

            equipmentDatabase.SwitchToNextShield();

            uIDocumentPlayerHUDV2.OnSwitchShield();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchConsumable()
        {
            if (!uIDocumentPlayerHUDV2.IsEquipmentDisplayed())
            {
                return;
            }

            equipmentDatabase.SwitchToNextConsumable();

            uIDocumentPlayerHUDV2.OnSwitchConsumable();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchSpell()
        {
            if (!uIDocumentPlayerHUDV2.IsEquipmentDisplayed())
            {
                return;
            }

            if (equipmentDatabase.IsBowEquipped())
            {
                equipmentDatabase.SwitchToNextArrow();
            }
            else
            {
                equipmentDatabase.SwitchToNextSpell();
            }

            uIDocumentPlayerHUDV2.OnSwitchSpell();
        }
    }
}
