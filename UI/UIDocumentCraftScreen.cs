using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AF.Music;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentCraftScreen : MonoBehaviour
    {
        public enum CraftActivity
        {
            ALCHEMY,
            COOKING,
            BLACKSMITH,
        }

        public CraftActivity craftActivity;

        [Header("UI")]
        public VisualTreeAsset recipeItem;
        public VisualTreeAsset ingredientItem;
        public Sprite alchemyBackgroundImage;
        public Sprite cookingBackgroundImage;
        public Sprite blacksmithBackgroundImage;

        public Sprite goldSprite;

        [Header("SFX")]
        public AudioClip sfxOnEnterMenu;

        [Header("UI Components")]
        public UIDocument uIDocument;
        [HideInInspector] public VisualElement root;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public NotificationManager notificationManager;
        public UIManager uiManager;
        public PlayerManager playerManager;
        public CursorManager cursorManager;
        public BGMManager bgmManager;
        public Soundbank soundbank;

        [HideInInspector] public bool returnToBonfire = false;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        int buttonIndexToFocusAfterRedraw;
        Button buttonToFocusAfterRedraw;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            bgmManager.PlaySound(sfxOnEnterMenu, null);
            cursorManager.ShowCursor();

            DrawUI();
        }

        private void OnDisable()
        {
            cursorManager.HideCursor();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenBlacksmithMenu()
        {
            this.craftActivity = CraftActivity.BLACKSMITH;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenAlchemyMenu()
        {
            this.craftActivity = CraftActivity.ALCHEMY;
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            if (returnToBonfire)
            {
                returnToBonfire = false;

                uIDocumentBonfireMenu.gameObject.SetActive(true);
                cursorManager.ShowCursor();
                this.gameObject.SetActive(false);
                return;
            }

            playerManager.playerComponentManager.EnableComponents();
            playerManager.playerComponentManager.EnableCharacterController();

            this.gameObject.SetActive(false);
            cursorManager.HideCursor();
        }

        void ClearPreviews()
        {
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            root.Q<Label>("WeaponLevelPreview").text = "";
            root.Q<Label>("WeaponLevelPreview").style.display = DisplayStyle.None;
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;
        }

        void SetupActivity()
        {
            string targetActivityTitleText = "";
            StyleBackground targetBackground = null;
            if (craftActivity == CraftActivity.ALCHEMY)
            {
                targetActivityTitleText = "Crafting Table";
                targetBackground = new StyleBackground(alchemyBackgroundImage);
            }
            else if (craftActivity == CraftActivity.BLACKSMITH)
            {
                targetActivityTitleText = "Weapon Upgrades";
                targetBackground = new StyleBackground(blacksmithBackgroundImage);
            }
            root.Q<VisualElement>("ImageBack").style.backgroundImage = targetBackground;
            root.Q<Label>("CraftActivityTitle").text = targetActivityTitleText;
        }

        void DrawUI()
        {
            ClearPreviews();

            SetupActivity();

            PopulateScrollView(recipesDatabase.craftingRecipes.ToArray());
        }

        void PopulateScrollView(CraftingRecipe[] ownedCraftingRecipes)
        {
            var scrollView = this.root.Q<ScrollView>();
            scrollView.Clear();

            Button exitButton = new()
            {
                text = "Return"
            };
            exitButton.AddToClassList("primary-button");
            UIUtils.SetupButton(exitButton, () =>
            {
                Close();
            }, soundbank);

            scrollView.nestedInteractionKind = ScrollView.NestedInteractionKind.ForwardScrolling;
            scrollView.Add(exitButton);

            if (craftActivity == CraftActivity.BLACKSMITH)
            {
                PopulateWeaponsScrollView();
            }
            else
            {
                PopulateCraftingScroll(scrollView, ownedCraftingRecipes);
            }

            if (buttonToFocusAfterRedraw != null)
            {
                buttonToFocusAfterRedraw.Focus();
                scrollView.ScrollTo(buttonToFocusAfterRedraw);
                buttonToFocusAfterRedraw = null;
            }
            else
            {
                exitButton.Focus();
            }
        }

        public string GetItemDescription(CraftingRecipe recipe)
        {
            string itemDescription = recipe.resultingItem.shortDescription?.Length > 0 ?
                                     recipe.resultingItem.shortDescription.Substring(0, System.Math.Min(60, recipe.resultingItem.shortDescription.Length)) : "";
            return itemDescription + (recipe.resultingItem.shortDescription?.Length > 60 ? "..." : "");
        }

        void MemoizeButtonToFocusAfterRedraw(int index, Button targetButton)
        {
            if (buttonIndexToFocusAfterRedraw == index)
            {
                buttonIndexToFocusAfterRedraw = -1;
                buttonToFocusAfterRedraw = targetButton;
            }
        }

        void PopulateCraftingScroll(ScrollView scrollView, CraftingRecipe[] ownedCraftingRecipes)
        {
            if (ownedCraftingRecipes.Length <= 0)
            {
                return;
            }

            int i = 0;
            foreach (var recipe in ownedCraftingRecipes)
            {
                i++;

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(recipe.resultingItem.sprite);
                scrollItem.Q<Label>("ItemName").text = recipe.resultingItem.name;
                scrollItem.Q<Label>("ItemDescription").text = GetItemDescription(recipe);

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");
                var craftLabel = scrollItem.Q<Label>("CraftLabel");

                craftLabel.text = GetCraftLabel();

                craftBtn.style.opacity = CraftingUtils.CanCraftItem(inventoryDatabase, recipe) ? 1f : 0.25f;

                UIUtils.SetupButton(craftBtn,
                () =>
                {
                    buttonIndexToFocusAfterRedraw = i;

                    if (!CraftingUtils.CanCraftItem(inventoryDatabase, recipe))
                    {
                        HandleCraftError("Missing ingredients!");
                        return;
                    }

                    if (ShouldRuinMixture(recipe))
                    {
                        HandleCraftError("Crafting failed! Try again...");
                        return;
                    }

                    HandleCraftSuccess(recipe);

                    DrawUI();
                },
                () =>
                {
                    ShowRequiredIngredients(recipe);
                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {

                },
                true,
                soundbank);

                MemoizeButtonToFocusAfterRedraw(i, craftBtn);

                scrollView.Add(scrollItem);
            }
        }

        void PopulateWeaponsScrollView()
        {
            var scrollView = this.root.Q<ScrollView>();

            int i = 0;
            foreach (var itemEntry in GetUpgradeableWeapons())
            {
                i++;

                Weapon wp = itemEntry.Key as Weapon;

                if (ShouldSkipUpgrade(wp, wp.level))
                {
                    continue;
                }

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(wp.sprite);
                scrollItem.Q<Label>("ItemName").text = GetWeaponName(wp);
                scrollItem.Q<Label>("ItemDescription").text = "";

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");
                craftBtn.style.opacity = CraftingUtils.CanImproveWeapon(inventoryDatabase, wp, playerStatsDatabase.gold) ? 1f : 0.25f;

                UIUtils.SetupButton(craftBtn, () =>
                {
                    buttonIndexToFocusAfterRedraw = i;

                    if (!CraftingUtils.CanImproveWeapon(inventoryDatabase, wp, playerStatsDatabase.gold))
                    {
                        HandleCraftError("Missing ingredients");
                        return;
                    }

                    HandleWeaponUpgrade(wp);

                    DrawUI();
                },
                () =>
                {
                    ShowRequirements(wp);
                    scrollView.ScrollTo(craftBtn);
                },
                () => { },
                true,
                soundbank);

                MemoizeButtonToFocusAfterRedraw(i, craftBtn);

                scrollView.Add(scrollItem);
            }
        }

        // Helper methods
        string GetCraftLabel()
        {
            return craftActivity switch
            {
                CraftActivity.ALCHEMY => "Craft",
                CraftActivity.COOKING => "Cook",
                CraftActivity.BLACKSMITH => "Upgrade",
                _ => "",
            };
        }

        void HandleCraftError(string errorMessage)
        {
            soundbank.PlaySound(soundbank.craftError);
            notificationManager.ShowNotification(errorMessage, notificationManager.alchemyLackOfIngredients);
        }

        bool ShouldRuinMixture(CraftingRecipe recipe)
        {
            var ingredientThatCanRuinMixture = recipe.ingredients.FirstOrDefault(x => x.ingredient.chanceToRuinMixture > 0);
            return ingredientThatCanRuinMixture != null && Random.Range(0, 100) < ingredientThatCanRuinMixture.ingredient.chanceToRuinMixture;
        }

        void HandleCraftSuccess(CraftingRecipe recipe)
        {
            if (craftActivity == CraftActivity.COOKING)
            {
                playerManager.playerAchievementsManager.achievementForCookingFirstMeal.AwardAchievement();
            }
            else if (craftActivity == CraftActivity.ALCHEMY)
            {
                playerManager.playerAchievementsManager.achievementForBrewingFirstPotion.AwardAchievement();
            }

            soundbank.PlaySound(soundbank.craftSuccess);
            playerManager.playerInventory.AddItem(recipe.resultingItem, 1);
            notificationManager.ShowNotification("Received " + recipe.resultingItem.name, recipe.resultingItem.sprite);

            foreach (var ingredient in recipe.ingredients)
            {
                playerManager.playerInventory.RemoveItem(ingredient.ingredient, ingredient.amount);
            }
        }

        Dictionary<Item, ItemAmount> GetUpgradeableWeapons()
        {
            return inventoryDatabase.ownedItems.Where(itemEntry => itemEntry.Key is Weapon wp && wp.canBeUpgraded).ToDictionary(item => item.Key, item => item.Value);
        }

        bool ShouldSkipUpgrade(Weapon wp, int nextLevel)
        {
            return wp.canBeUpgraded == false || nextLevel >= wp.weaponUpgrades.Count();
        }

        string GetWeaponName(Weapon wp)
        {
            return $"{wp.name} +{wp.level} > {wp.name} +{wp.level + 1}";
        }

        void HandleWeaponUpgrade(Weapon wp)
        {
            playerManager.playerAchievementsManager.achievementForUpgradingFirstWeapon.AwardAchievement();
            soundbank.PlaySound(soundbank.craftSuccess);
            notificationManager.ShowNotification("Weapon improved!", wp.sprite);

            CraftingUtils.UpgradeWeapon(
                wp,
                (goldUsed) => uIDocumentPlayerGold.LoseGold(goldUsed),
                (upgradeMaterialUsed) => playerManager.playerInventory.RemoveItem(upgradeMaterialUsed.Key, upgradeMaterialUsed.Value)
            );
        }

        void ShowRequiredIngredients(CraftingRecipe recipe)
        {
            root.Q<VisualElement>("ItemInfo").Clear();

            foreach (var ingredient in recipe.ingredients)
            {
                var ingredientItemEntry = ingredientItem.CloneTree();
                ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(ingredient.ingredient.sprite);
                ingredientItemEntry.Q<Label>("Title").text = ingredient.ingredient.name;

                var playerOwnedIngredientAmount = 0;

                var playerOwnedIngredient = inventoryDatabase.HasItem(ingredient.ingredient)
                    ? inventoryDatabase.ownedItems[ingredient.ingredient]
                    : null;

                if (playerOwnedIngredient != null)
                {
                    playerOwnedIngredientAmount = playerOwnedIngredient.amount;
                }

                ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + ingredient.amount;
                ingredientItemEntry.Q<Label>("Amount").style.opacity = playerOwnedIngredientAmount >= ingredient.amount ? 1 : 0.25f;

                root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);
            }

            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 1;
        }
        void ShowRequirements(Weapon weapon)
        {
            WeaponUpgradeLevel weaponUpgradeLevel = weapon.weaponUpgrades.ElementAtOrDefault(weapon.level - 1);

            if (weaponUpgradeLevel == null)
            {
                return;
            }

            var nextLevel = weapon.level + 1;

            // Weapon preview
            root.Q<Label>("WeaponLevelPreview").text = weapon.name + " +" + nextLevel;
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;

            if (weapon.physicalAttack > 0)
            {
                root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("PhysicalAttack").text = "Next Physical Damage: "
                    + weapon.GetWeaponAttackForLevel(weapon.level) + " > " + weapon.GetWeaponAttackForLevel(nextLevel);
            }
            if (weapon.fireAttack > 0)
            {
                root.Q<Label>("FireAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("FireAttack").text = "Next Fire Bonus: "
                    + weapon.GetWeaponFireAttackForLevel(weapon.level) + " > " + weapon.GetWeaponFireAttackForLevel(nextLevel);
            }
            if (weapon.frostAttack > 0)
            {
                root.Q<Label>("FrostAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("FrostAttack").text = "Next Frost Bonus: "
                    + weapon.GetWeaponFrostAttackForLevel(weapon.level) + " > " + weapon.GetWeaponFrostAttackForLevel(nextLevel);
            }
            if (weapon.lightningAttack > 0)
            {
                root.Q<Label>("LightningAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("LightningAttack").text = "Next Lightning Bonus: "
                    + weapon.GetWeaponLightningAttackForLevel(weapon.level) + " > " + weapon.GetWeaponLightningAttackForLevel(nextLevel);
            }
            if (weapon.magicAttack > 0)
            {
                root.Q<Label>("MagicAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("MagicAttack").text = "Next Magic Bonus: "
                    + weapon.GetWeaponMagicAttackForLevel(weapon.level) + " > " + weapon.GetWeaponMagicAttackForLevel(nextLevel);
            }

            // Requirements

            root.Q<VisualElement>("ItemInfo").Clear();

            foreach (var upgradeMaterial in weaponUpgradeLevel.upgradeMaterials)
            {
                UpgradeMaterial upgradeMaterialItem = upgradeMaterial.Key;
                int amountRequiredFoUpgrade = upgradeMaterial.Value;

                var ingredientItemEntry = ingredientItem.CloneTree();
                ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(upgradeMaterialItem.sprite);
                ingredientItemEntry.Q<Label>("Title").text = upgradeMaterialItem.name;

                var playerOwnedIngredient = inventoryDatabase.HasItem(upgradeMaterialItem)
                    ? inventoryDatabase.ownedItems[upgradeMaterialItem]
                    : null;

                var playerOwnedIngredientAmount = 0;
                if (playerOwnedIngredient != null)
                {
                    playerOwnedIngredientAmount = playerOwnedIngredient.amount;
                }
                ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + amountRequiredFoUpgrade;
                ingredientItemEntry.Q<Label>("Amount").style.opacity =
                    playerOwnedIngredient != null && playerOwnedIngredientAmount >= amountRequiredFoUpgrade ? 1 : 0.25f;

                root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);
            }

            // Add Gold

            var goldItemEntry = ingredientItem.CloneTree();
            goldItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(goldSprite);
            goldItemEntry.Q<Label>("Title").text = "Gold";

            goldItemEntry.Q<Label>("Amount").text = playerStatsDatabase.gold + " / " + weaponUpgradeLevel.goldCostForUpgrade;
            goldItemEntry.Q<Label>("Amount").style.opacity = playerStatsDatabase.gold >= weaponUpgradeLevel.goldCostForUpgrade ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(goldItemEntry);
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 1;
        }
    }
}
