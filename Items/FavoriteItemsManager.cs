using UnityEngine;
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

        [Header("Components")]
        public PlayerManager playerManager;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnConsumableUse()
        {
            if (!CanSwitchEquipment())
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
            if (!CanSwitchEquipment())
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
            if (!CanSwitchEquipment())
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
            if (!CanSwitchEquipment())
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
            if (!CanSwitchEquipment())
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

        bool CanSwitchEquipment()
        {
            if (!uIDocumentPlayerHUDV2.IsEquipmentDisplayed())
            {
                return false;
            }

            if (playerManager.isBusy)
            {
                return false;
            }

            return true;
        }
    }
}
