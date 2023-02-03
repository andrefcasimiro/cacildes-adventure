using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerInventory : MonoBehaviour
    {
        public readonly int hashDrinking = Animator.StringToHash("Drinking");
        public readonly int hashEating = Animator.StringToHash("Eating");
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
        PlayerPoiseController playerPoiseController => GetComponent<PlayerPoiseController>();

        public Transform handItemRef;

        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        public void AddItem(Item item, int quantity)
        {
            Player.ItemEntry itemEntry = new Player.ItemEntry();
            itemEntry.item = item;
            itemEntry.amount = quantity;

            var idx = Player.instance.ownedItems.FindIndex(x => x.item == item);
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
            int itemEntryIndex = Player.instance.ownedItems.FindIndex(_itemEntry => _itemEntry.item == item);

            if (itemEntryIndex == -1)
            {
                return;
            }

            if (Player.instance.ownedItems[itemEntryIndex].amount <= 1)
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
                        favoriteItemsManager.SwitchFavoriteItemsOrder();
                    }
                }
            }
            else
            {
                Player.instance.ownedItems[itemEntryIndex].amount -= amount;
            }

            FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();
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

            if (playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification("Can't consume item while attacking", notificationManager.systemError);
                return;
            }

            if (playerParryManager.IsBlocking())
            {
                animator.SetBool(playerParryManager.hashIsBlocking, false);

            }

            if (playerPoiseController.isStunned)
            {
                notificationManager.ShowNotification("Can't consume item while stunned", notificationManager.systemError);
                return;
            }

            if (playerPoiseController.IsTakingDamage())
            {
                notificationManager.ShowNotification("Can't consume item while taking damage", notificationManager.systemError);
                return;
            }

            if (dodgeController.IsDodging())
            {
                notificationManager.ShowNotification("Can't consume item while dodging", notificationManager.systemError);
                return;
            }

            if (!thirdPersonController.Grounded)
            {
                notificationManager.ShowNotification("Can't consume item while on air", notificationManager.systemError);
                return;
            }

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                notificationManager.ShowNotification("Can't consume item while climbing", notificationManager.systemError);
                return;
            }

            if (Player.instance.currentHealth <= 0)
            {
                return;
            }

            this.currentConsumedItem = consumable;

            if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.DRINK)
            {
                animator.Play(hashDrinking);
            }
            else if (consumable.onConsumeActionType == Consumable.OnConsumeActionType.EAT)
            {
                animator.Play(hashEating);
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
