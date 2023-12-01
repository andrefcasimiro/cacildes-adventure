using System.Collections;
using System.Collections.Generic;
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

        public void ResetStates()
        {
            isConsumingItem = false;
        }

        public void ReplenishItems()
        {
            foreach (var item in inventoryDatabase.ownedItems)
            {
                if (item.usages > 0)
                {
                    var idx = inventoryDatabase.ownedItems.IndexOf(item);

                    inventoryDatabase.ownedItems[idx].amount += item.usages;
                    inventoryDatabase.ownedItems[idx].usages = 0;
                }
            }

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        void HandleItemAchievements(Item item)
        {
            if (item is Weapon)
            {
                int numberOfWeapons = inventoryDatabase.ownedItems.Count(x => x.item is Weapon);

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
                int numberOfSpells = inventoryDatabase.ownedItems.Count(x => x.item is Spell);

                if (numberOfSpells <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstSpell.AwardAchievement();
                }
            }
        }

        public void AddItem(Item item, int quantity)
        {
            HandleItemAchievements(item);

            ItemEntry itemEntry = new()
            {
                item = item,
                amount = quantity
            };

            var idx = inventoryDatabase.ownedItems.FindIndex(x => x.item.name.GetEnglishText() == item.name.GetEnglishText());
            if (idx != -1)
            {
                inventoryDatabase.ownedItems[idx].amount += itemEntry.amount;
            }
            else
            {
                inventoryDatabase.ownedItems.Add(itemEntry);
            }

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        public void RemoveItem(Item item, int amount)
        {
            int itemEntryIndex = inventoryDatabase.ownedItems.FindIndex(_itemEntry => _itemEntry.item.name.GetEnglishText() == item.name.GetEnglishText());

            if (itemEntryIndex == -1)
            {
                return;
            }

            if (inventoryDatabase.ownedItems[itemEntryIndex].amount <= 1)
            {
                // If not reusable item
                if (item.lostUponUse)
                {
                    // Remove item 
                    inventoryDatabase.ownedItems.RemoveAt(itemEntryIndex);
                }
                else
                {
                    inventoryDatabase.ownedItems[itemEntryIndex].amount = 0;
                    inventoryDatabase.ownedItems[itemEntryIndex].usages++;
                }
            }
            else
            {
                inventoryDatabase.ownedItems[itemEntryIndex].amount -= amount;

                if (item.lostUponUse == false)
                {
                    inventoryDatabase.ownedItems[itemEntryIndex].usages++;
                }

            }

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        public int GetItemQuantity(Item item)
        {
            var entry = inventoryDatabase.ownedItems.FirstOrDefault(x => x.item.name.GetEnglishText() == item.name.GetEnglishText());

            if (entry == null)
            {
                return 0;
            }

            return entry.amount;
        }

        public void PrepareItemForConsuming(Consumable consumable)
        {
            if (isConsumingItem)
            {
                return;
            }

            if (consumable.lostUponUse == false && inventoryDatabase.GetItemAmount(consumable) <= 0)
            {
                notificationManager.ShowNotification(LocalizedTerms.DepletedConsumable(), notificationManager.notEnoughSpells);
                return;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }


            if (playerManager.playerPoiseController.isStunned)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (playerManager.playerPoiseController.IsTakingDamage())
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (playerManager.dodgeController.IsDodging())
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (!playerManager.thirdPersonController.Grounded)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
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

                if (menuManager.IsMenuOpen())
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
            if (currentConsumedItem.onConsumeActionType == Consumable.OnConsumeActionType.CLOCK)
            {
                StartCoroutine(RecoverWatchControl(1.2f));
                return;
            }

            if (currentConsumedItem.destroyItemOnConsumeMoment)
            {
                StartCoroutine(RecoverControl(currentConsumedItem.onConsumeActionType == Consumable.OnConsumeActionType.DRINK ? 0.7f : 0.9f));
            }
            else
            {
                StartCoroutine(RecoverControl(0f));
            }

            currentConsumedItem = null;
            Destroy(this.consumableGraphicInstance);
        }


        IEnumerator RecoverControl(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            playerManager.playerComponentManager.EnableCharacterController();
            playerManager.playerComponentManager.EnableComponents();
        }

        IEnumerator RecoverWatchControl(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            playerManager.playerComponentManager.EnableCharacterController();
            playerManager.playerComponentManager.EnableComponents();

            currentConsumedItem = null;
            Destroy(this.consumableGraphicInstance);
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void OnItemConsumed()
        {
            if (currentConsumedItem == null)
            {
                return;
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                return;
            }

            if (currentConsumedItem.sfxOnConsume != null)
            {
                BGMManager.instance.PlaySound(currentConsumedItem.sfxOnConsume, null);
            }

            if (currentConsumedItem.particleOnConsume != null)
            {
                Instantiate(currentConsumedItem.particleOnConsume, GameObject.FindWithTag("Player").transform);
            }


            //      currentConsumedItem.OnConsumeSuccess(this, statusDatabase);
            /*
                        if (currentConsumedItem.destroyItemOnConsumeMoment)
                        {
                            FinishItemConsumption();
                        }*/
        }
    }

}
