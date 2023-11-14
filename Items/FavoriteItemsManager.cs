using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TigerForge;
using AF.Events;
using AF.Inventory;

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

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnConsumableUse()
        {
            if (playerStatsDatabase.currentHealth <= 0)
            {
                return;
            }

            Item currentItem = equipmentDatabase.GetCurrentConsumable();

            if (currentItem == null)
            {
                return;
            }

            ItemEntry itemEntry = inventoryDatabase.ownedItems.Find(item => item.item.name.GetEnglishText() == currentItem.name.GetEnglishText());

            if (itemEntry.amount <= 1 && itemEntry.item.lostUponUse)
            {
                equipmentDatabase.UnequipConsumable(equipmentDatabase.currentConsumableIndex);
            }

            Consumable consumableItem = currentItem as Consumable;
            if (consumableItem != null)
            {
                consumableItem.OnConsume();
            }

            uIDocumentPlayerHUDV2.UpdateFavoriteItems();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchWeapon()
        {
            equipmentDatabase.SwitchToNextWeapon();

            uIDocumentPlayerHUDV2.OnSwitchWeapon();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchShield()
        {
            equipmentDatabase.SwitchToNextShield();

            uIDocumentPlayerHUDV2.OnSwitchShield();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchConsumable()
        {
            equipmentDatabase.SwitchToNextConsumable();

            uIDocumentPlayerHUDV2.OnSwitchConsumable();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnSwitchSpell()
        {
            equipmentDatabase.SwitchToNextSpell();

            uIDocumentPlayerHUDV2.OnSwitchSpell();
        }
    }
}
