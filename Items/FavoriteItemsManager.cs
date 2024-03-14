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

        bool canSwitch = true;

        void ResetCanSwitchFlag()
        {
            canSwitch = true;
        }

        void UpdateCanSwitchFlag()
        {
            canSwitch = false;
            Invoke(nameof(ResetCanSwitchFlag), 0.1f);
        }

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

            if (itemAmount <= 1 && !currentItem.isRenewable)
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

            UpdateCanSwitchFlag();
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

            UpdateCanSwitchFlag();
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
            UpdateCanSwitchFlag();
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
            UpdateCanSwitchFlag();
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

            if (canSwitch == false)
            {
                return false;
            }

            return true;
        }
    }
}
