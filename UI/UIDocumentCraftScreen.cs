using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
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

        [HideInInspector] public bool returnToBonfire = false;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            BGMManager.instance.PlaySound(sfxOnEnterMenu, null);
            cursorManager.ShowCursor();

            DrawUI();
        }

        private void OnDisable()
        {
            cursorManager.HideCursor();
        }

        void Close()
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

        void DrawUI()
        {
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            // Clear Weapon preview
            root.Q<Label>("WeaponLevelPreview").text = "";
            root.Q<Label>("WeaponLevelPreview").style.display = DisplayStyle.None;
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;

            var craftActivityTitle = root.Q<Label>("CraftActivityTitle");

            if (craftActivity == CraftActivity.ALCHEMY)
            {
                root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(alchemyBackgroundImage);

                craftActivityTitle.text = "Crafting Table";
                PopulateScrollView(recipesDatabase.alchemyRecipes.ToArray());
            }
            else if (craftActivity == CraftActivity.COOKING)
            {
                root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(cookingBackgroundImage);

                craftActivityTitle.text = "Chef's Table";
                PopulateScrollView(recipesDatabase.cookingRecipes.ToArray());
            }
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
            });

            exitButton.Focus();

            scrollView.Add(exitButton);

            if (craftActivity == CraftActivity.BLACKSMITH)
            {
                PopulateWeaponsScrollView();
            }
            else
            {
                PopulateCraftingScroll(scrollView, ownedCraftingRecipes);
            }
        }

        void PopulateCraftingScroll(ScrollView scrollView, CraftingRecipe[] ownedCraftingRecipes)
        {
            if (ownedCraftingRecipes.Length <= 0) { return; }

            int i = 0;
            foreach (var recipe in ownedCraftingRecipes)
            {
                i++;

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(recipe.resultingItem.sprite);
                scrollItem.Q<Label>("ItemName").text = recipe.resultingItem.name.GetText();
                scrollItem.Q<Label>("ItemDescription").text = recipe.resultingItem.shortDescription.localizedTexts.Length > 0 ? recipe.resultingItem.shortDescription.GetText() : "";

                var craftBtn = scrollItem.Q<Button>("CraftButton");

                craftBtn.text = "";
                if (craftActivity == CraftActivity.ALCHEMY)
                {
                    craftBtn.text = "Craft";
                }
                else if (craftActivity == CraftActivity.COOKING)
                {
                    craftBtn.text = "Cook";
                }
                else if (craftActivity == CraftActivity.BLACKSMITH)
                {
                    craftBtn.text = "Upgrade";
                }

                if (CanCraftItem(recipe))
                {
                    craftBtn.style.opacity = 1f;
                }
                else
                {
                    craftBtn.style.opacity = 0.25f;
                }

                UIUtils.SetupButton(craftBtn, () =>
                {
                    if (craftBtn.enabledSelf == false)
                    {
                        return;
                    }

                    if (!CanCraftItem(recipe))
                    {
                        Soundbank.instance.PlayCraftError();
                        notificationManager.ShowNotification("Missing ingredients!", notificationManager.alchemyLackOfIngredients);
                        return;
                    }

                    var ingredientThatCanRuinMixture = recipe.ingredients.FirstOrDefault(x => x.ingredient.chanceToRuinMixture > 0);
                    float chanceToRuinMixture = 0;
                    if (ingredientThatCanRuinMixture != null)
                    {
                        chanceToRuinMixture = ingredientThatCanRuinMixture.ingredient.chanceToRuinMixture;
                    }

                    if (chanceToRuinMixture > 0 && Random.Range(0, 100) < chanceToRuinMixture)
                    {
                        Soundbank.instance.PlayCraftError();
                        notificationManager.ShowNotification("Crafting failed! Try again...", notificationManager.alchemyLackOfIngredients);
                    }
                    else
                    {
                        if (craftActivity == CraftActivity.COOKING)
                        {
                            playerManager.playerAchievementsManager.achievementForCookingFirstMeal.AwardAchievement();
                        }
                        else if (craftActivity == CraftActivity.ALCHEMY)
                        {
                            playerManager.playerAchievementsManager.achievementForBrewingFirstPotion.AwardAchievement();
                        }


                        Soundbank.instance.PlayCraftSuccess();
                        playerManager.playerInventory.AddItem(recipe.resultingItem, 1);
                        notificationManager.ShowNotification("Received " + " " + recipe.resultingItem.name.GetText(), recipe.resultingItem.sprite);

                    }

                    foreach (var ingredient in recipe.ingredients)
                    {
                        playerManager.playerInventory.RemoveItem(ingredient.ingredient, ingredient.amount);
                    }

                    DrawUI();
                },
                () =>
                {
                    ShowRequiredIngredients(recipe);

                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                },
                true);

                scrollView.Add(scrollItem);
            }
        }

        void PopulateWeaponsScrollView()
        {
            var scrollView = this.root.Q<ScrollView>();
            scrollView.Clear();

            List<ItemEntry> weapons = new();

            foreach (var it in inventoryDatabase.ownedItems)
            {
                Weapon wp = it.item as Weapon;
                if (wp != null && wp.canBeUpgraded)
                {
                    weapons.Add(it);
                }
            }

            foreach (var itemEntry in weapons)
            {
                Weapon wp = itemEntry.item as Weapon;
                var nextLevel = wp.level;
                nextLevel++;

                if (wp.upgradeMaterial == null || nextLevel >= 11)
                {
                    continue;
                }

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(wp.sprite);
                scrollItem.Q<Label>("ItemName").text = wp.name.GetText() + (wp.level > 1 ? "+" + wp.level : "");
                scrollItem.Q<Label>("ItemDescription").text = "";

                var craftBtn = scrollItem.Q<Button>("CraftButton");
                craftBtn.text = "Upgrade";

                if (CanImproveWeapon(wp))
                {
                    craftBtn.style.opacity = 1f;
                }
                else
                {
                    craftBtn.style.opacity = 0.25f;
                }

                UIUtils.SetupButton(craftBtn, () =>
                {
                    if (!CanImproveWeapon(wp))
                    {
                        Soundbank.instance.PlayCraftError();
                        notificationManager.ShowNotification("Missing ingredients", notificationManager.alchemyLackOfIngredients);
                        return;
                    }

                    playerManager.playerAchievementsManager.achievementForUpgradingFirstWeapon.AwardAchievement();

                    SteamAPI.instance.SetAchievementProgress(SteamAPI.AchievementName.WEAPONSMITH, 1);

                    Soundbank.instance.PlayCraftSuccess();

                    notificationManager.ShowNotification("Weapon improved!", wp.sprite);

                    var currentWeaponLevel = wp.level;
                    uIDocumentPlayerGold.LoseGold(wp.GetRequiredUpgradeGoldForGivenLevel(currentWeaponLevel + 1));
                    playerManager.playerInventory.RemoveItem(wp.upgradeMaterial, wp.GetRequiredOresForGivenLevel(currentWeaponLevel + 1));

                    wp.level++;


                    DrawUI();
                },
                () =>
                {
                    ShowRequirements(wp);

                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                },
                true);

                scrollView.Add(scrollItem);
            }
        }


        void ShowRequiredIngredients(CraftingRecipe recipe)
        {
            root.Q<VisualElement>("ItemInfo").Clear();

            foreach (var ingredient in recipe.ingredients)
            {
                var ingredientItemEntry = ingredientItem.CloneTree();
                ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(ingredient.ingredient.sprite);
                ingredientItemEntry.Q<Label>("Title").text = ingredient.ingredient.name.GetText();

                var playerOwnedIngredient = inventoryDatabase.ownedItems.Find(x => x.item.name.GetEnglishText() == ingredient.ingredient.name.GetEnglishText());
                var playerOwnedIngredientAmount = 0;
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

        bool CanCraftItem(CraftingRecipe recipe)
        {
            bool hasEnoughMaterial = true;

            foreach (var ingredient in recipe.ingredients)
            {
                var itemEntry = inventoryDatabase.ownedItems.Find(x => x.item.name.GetEnglishText() == ingredient.ingredient.name.GetEnglishText());

                if (itemEntry == null)
                {
                    hasEnoughMaterial = false;
                    break;
                }

                if (itemEntry.amount >= ingredient.amount)
                {
                    hasEnoughMaterial = true;
                }
                else
                {
                    hasEnoughMaterial = false;
                    break;
                }
            }

            return hasEnoughMaterial;
        }

        void ShowRequirements(Weapon weapon)
        {

            var nextLevel = weapon.level;
            nextLevel++;

            if (weapon.upgradeMaterial == null)
            {
                return;
            }

            // Weapon preview
            root.Q<Label>("WeaponLevelPreview").text = weapon.name.GetText() + " +" + (nextLevel);
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;

            if (weapon.physicalAttack > 0)
            {
                root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("PhysicalAttack").text = LocalizedTerms.PhysicalDamage() + ": " + weapon.GetWeaponAttackForLevel(weapon.level) + " > " + weapon.GetWeaponAttackForLevel(nextLevel);
            }
            if (weapon.fireAttack > 0)
            {
                root.Q<Label>("FireAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("FireAttack").text = LocalizedTerms.FireBonus() + ": " + weapon.GetWeaponFireAttackForLevel(weapon.level) + " > " + weapon.GetWeaponFireAttackForLevel(nextLevel);
            }
            if (weapon.frostAttack > 0)
            {
                root.Q<Label>("FrostAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("FrostAttack").text = LocalizedTerms.FrostBonus() + ": " + weapon.GetWeaponFrostAttackForLevel(weapon.level) + " > " + weapon.GetWeaponFrostAttackForLevel(nextLevel);
            }
            if (weapon.lightningAttack > 0)
            {
                root.Q<Label>("LightningAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("LightningAttack").text = LocalizedTerms.LightningBonus() + ": " + weapon.GetWeaponLightningAttackForLevel(weapon.level) + " > " + weapon.GetWeaponLightningAttackForLevel(nextLevel);
            }
            if (weapon.magicAttack > 0)
            {
                root.Q<Label>("MagicAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("MagicAttack").text = LocalizedTerms.MagicBonus() + ": " + weapon.GetWeaponMagicAttackForLevel(weapon.level) + " > " + weapon.GetWeaponMagicAttackForLevel(nextLevel);
            }

            // Requiremnts
            root.Q<VisualElement>("ItemInfo").Clear();

            var ingredientItemEntry = ingredientItem.CloneTree();
            ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(weapon.upgradeMaterial.sprite);
            ingredientItemEntry.Q<Label>("Title").text = weapon.upgradeMaterial.name.GetText();

            var playerOwnedIngredient = inventoryDatabase.ownedItems.Find(x => x.item.name.GetEnglishText() == weapon.upgradeMaterial.name.GetEnglishText());
            var playerOwnedIngredientAmount = 0;
            if (playerOwnedIngredient != null)
            {
                playerOwnedIngredientAmount = playerOwnedIngredient.amount;
            }
            ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + weapon.GetRequiredOresForGivenLevel(nextLevel);
            ingredientItemEntry.Q<Label>("Amount").style.opacity = playerOwnedIngredient != null && playerOwnedIngredientAmount >= playerOwnedIngredient.amount ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);

            // Add Gold

            var goldItemEntry = ingredientItem.CloneTree();
            goldItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(goldSprite);
            goldItemEntry.Q<Label>("Title").text = LocalizedTerms.Gold();

            goldItemEntry.Q<Label>("Amount").text = playerStatsDatabase.gold + " / " + weapon.GetRequiredUpgradeGoldForGivenLevel(nextLevel);
            goldItemEntry.Q<Label>("Amount").style.opacity = playerStatsDatabase.gold >= weapon.GetRequiredUpgradeGoldForGivenLevel(nextLevel) ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(goldItemEntry);
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 1;
        }

        bool CanImproveWeapon(Weapon weapon)
        {
            var nextLevel = weapon.level;
            nextLevel++;

            if (weapon.upgradeMaterial == null)
            {
                return false;
            }

            var itemEntry = inventoryDatabase.ownedItems.Find(x => x.item.name.GetEnglishText() == weapon.upgradeMaterial.name.GetEnglishText());

            if (itemEntry == null)
            {
                return false;
            }

            if (itemEntry.amount < weapon.GetRequiredOresForGivenLevel(nextLevel))
            {
                return false;
            }

            if (playerStatsDatabase.gold < weapon.GetRequiredUpgradeGoldForGivenLevel(nextLevel))
            {
                return false;
            }

            return true;
        }
    }
}
