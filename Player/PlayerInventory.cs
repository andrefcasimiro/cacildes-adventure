using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class PlayerInventory : MonoBehaviour
    {
        public readonly int hashDrinking = Animator.StringToHash("Drinking");
        public readonly int hashEating = Animator.StringToHash("Eating");
        public readonly int hashClock = Animator.StringToHash("Clock");
        public readonly int hashIsConsumingItem = Animator.StringToHash("IsConsumingItem");

        FavoriteItemsManager favoriteItemsManager => GetComponent<FavoriteItemsManager>();

        public Consumable currentConsumedItem;
        GameObject consumableGraphicInstance;

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        Animator animator => GetComponent<Animator>();

        PlayerComponentManager playerComponentManager => GetComponent<PlayerComponentManager>();

        DodgeController dodgeController => GetComponent<DodgeController>();
        ClimbController climbController => GetComponent<ClimbController>();
        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();
        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        PlayerParryManager playerParryManager => GetComponent<PlayerParryManager>();
        PlayerInventory playerInventory => GetComponent<PlayerInventory>();
        PlayerPoiseController playerPoiseController => GetComponent<PlayerPoiseController>();

        public Transform handItemRef;

        NotificationManager notificationManager;

        ViewClockMenu viewClockMenu;

        [SerializeField] private UIManager uIManager;
        [SerializeField] private MenuManager menuManager;

        [Header("Components")]
        public PlayerAchievementsManager playerAchievementsManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);

            viewClockMenu = FindAnyObjectByType<ViewClockMenu>(FindObjectsInactive.Include);
        }

        public void ReplenishItems()
        {
            foreach (var item in Player.instance.ownedItems)
            {
                if (item.usages > 0)
                {
                    var idx = Player.instance.ownedItems.IndexOf(item);

                    Player.instance.ownedItems[idx].amount += item.usages;
                    Player.instance.ownedItems[idx].usages = 0;
                }
            }

            favoriteItemsManager.UpdateFavoriteItems();
        }

        void HandleItemAchievements(Item item)
        {
            if (item is Weapon)
            {
                int numberOfWeapons = Player.instance.ownedItems.Count(x => x.item is Weapon);

                if (numberOfWeapons <= 0)
                {
                    playerAchievementsManager.achievementOnAcquiringFirstWeapon.AwardAchievement();
                }
                else if (numberOfWeapons == 10)
                {
                    playerAchievementsManager.achievementOnAcquiringTenWeapons.AwardAchievement();
                }
            }
            else if (item is Spell)
            {
                int numberOfSpells = Player.instance.ownedItems.Count(x => x.item is Spell);

                if (numberOfSpells <= 0)
                {
                    playerAchievementsManager.achievementOnAcquiringFirstSpell.AwardAchievement();
                }
            }
        }

        public void AddItem(Item item, int quantity)
        {
            HandleItemAchievements(item);

            Player.ItemEntry itemEntry = new()
            {
                item = item,
                amount = quantity
            };

            var idx = Player.instance.ownedItems.FindIndex(x => x.item.name.GetEnglishText() == item.name.GetEnglishText());
            if (idx != -1)
            {
                Player.instance.ownedItems[idx].amount += itemEntry.amount;
            }
            else
            {
                Player.instance.ownedItems.Add(itemEntry);
            }

            FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
        }

        public void RemoveItem(Item item, int amount)
        {
            int itemEntryIndex = Player.instance.ownedItems.FindIndex(_itemEntry => _itemEntry.item.name.GetEnglishText() == item.name.GetEnglishText());

            if (itemEntryIndex == -1)
            {
                return;
            }

            if (Player.instance.ownedItems[itemEntryIndex].amount <= 1)
            {
                // If not reusable item
                if (item.lostUponUse)
                {
                    // Remove item 
                    Player.instance.ownedItems.RemoveAt(itemEntryIndex);

                    // Remove item from favorite
                    var idxOfThisItemInFavorites = Player.instance.favoriteItems.IndexOf(item);
                    if (idxOfThisItemInFavorites != -1)
                    {
                        Player.instance.favoriteItems.Remove(item);

                        if (idxOfThisItemInFavorites == 0)
                        {
                            favoriteItemsManager.SwitchFavoriteItemsOrder(1);
                        }
                    }
                }
                else
                {
                    Player.instance.ownedItems[itemEntryIndex].amount = 0;
                    Player.instance.ownedItems[itemEntryIndex].usages++;
                }
            }
            else
            {
                Player.instance.ownedItems[itemEntryIndex].amount -= amount;

                if (item.lostUponUse == false)
                {
                    Player.instance.ownedItems[itemEntryIndex].usages++;
                }

            }

            FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
        }

        public int GetItemQuantity(Item item)
        {
            var entry = Player.instance.ownedItems.FirstOrDefault(x => x.item.name.GetEnglishText() == item.name.GetEnglishText());

            if (entry == null)
            {
                return 0;
            }

            return entry.amount;
        }

        public List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleWeapon = item.item as Weapon;
                if (possibleWeapon != null)
                {
                    weapons.Add(possibleWeapon);
                }
            }

            return weapons;
        }

        public List<Shield> GetShields()
        {
            List<Shield> shields = new List<Shield>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleShield = item.item as Shield;
                if (possibleShield != null)
                {
                    shields.Add(possibleShield);
                }
            }

            return shields;
        }

        public List<Accessory> GetAccessories()
        {
            List<Accessory> accessories = new List<Accessory>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleAccessory = item.item as Accessory;
                if (possibleAccessory != null)
                {
                    accessories.Add(possibleAccessory);
                }
            }

            return accessories;
        }

        public List<Helmet> GetHelmets()
        {
            List<Helmet> items = new List<Helmet>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Helmet;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public List<Armor> GetArmors()
        {
            List<Armor> items = new List<Armor>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Armor;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public List<Legwear> GetLegwears()
        {
            List<Legwear> items = new List<Legwear>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Legwear;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }


        public List<Gauntlet> GetGauntlets()
        {
            List<Gauntlet> items = new List<Gauntlet>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Gauntlet;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public List<Consumable> GetConsumables()
        {
            List<Consumable> items = new List<Consumable>();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Consumable;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public List<Spell> GetSpells()
        {
            List<Spell> items = new();

            foreach (var item in Player.instance.ownedItems)
            {
                var possibleItem = item.item as Spell;
                if (possibleItem != null)
                {
                    items.Add(possibleItem);
                }
            }

            return items;
        }

        public bool IsConsumingItem()
        {
            return animator.GetBool(hashIsConsumingItem);
        }

        public void PrepareItemForConsuming(Consumable consumable)
        {
            if (IsConsumingItem())
            {
                return;
            }


            if (consumable.lostUponUse == false && playerInventory.GetItemQuantity(consumable) <= 0)
            {
                notificationManager.ShowNotification(LocalizedTerms.DepletedConsumable(), notificationManager.notEnoughSpells);
                return;
            }

            if (playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (playerParryManager.IsBlocking())
            {
                animator.SetBool(playerParryManager.hashIsBlocking, false);

            }

            if (playerPoiseController.isStunned)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (playerPoiseController.IsTakingDamage())
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (dodgeController.IsDodging())
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (!thirdPersonController.Grounded)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                notificationManager.ShowNotification(LocalizedTerms.CantConsumeAtThisTime(), notificationManager.systemError);
                return;
            }

            if (playerComponentManager.isInBonfire)
            {
                return;
            }

            if (Player.instance.currentHealth <= 0)
            {
                return;
            }


            this.currentConsumedItem = consumable;

            if (consumable.isAlcoholic)
            {
                playerAchievementsManager.achievementForDrinkingFirstAlcoholicBeverage.AwardAchievement();
            }

            if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.DRINK)
            {
                animator.Play(hashDrinking);
            }
            else if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.EAT)
            {
                animator.Play(hashEating);
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

            equipmentGraphicsHandler.HideWeapons();

            if (consumable.graphic != null)
            {
                consumableGraphicInstance = Instantiate(consumable.graphic, handItemRef);
            }

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();
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

            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();
        }

        IEnumerator RecoverWatchControl(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();

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

            if (currentConsumedItem.sfxOnConsume != null)
            {
                BGMManager.instance.PlaySound(currentConsumedItem.sfxOnConsume, null);
            }

            if (currentConsumedItem.particleOnConsume != null)
            {
                Instantiate(currentConsumedItem.particleOnConsume, GameObject.FindWithTag("Player").transform);
            }

            currentConsumedItem.OnConsumeSuccess();

            if (currentConsumedItem.destroyItemOnConsumeMoment)
            {
                FinishItemConsumption();
            }
        }
    }

}
