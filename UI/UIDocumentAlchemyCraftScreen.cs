using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentAlchemyCraftScreen : MonoBehaviour
    {
        CursorManager cursorManager;

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

        public AudioClip sfxOnEnterMenu;

        [Header("Localization")]
        public LocalizedText ButtonExit;
        public LocalizedText AlchemyActivityTitle;
        public LocalizedText CookingActivityTitle;
        public LocalizedText AvailableRecipes;
        public LocalizedText CraftButton;
        public LocalizedText missingIngredientsMessage;
        public LocalizedText receivedMessage;
        public LocalizedText failedMixtureMessage;
        public LocalizedText requiredIngredients;

        UIDocument uIDocument => GetComponent<UIDocument>();
        [HideInInspector] public VisualElement root;

        NotificationManager notificationManager;
        public UIManager uiManager;
        PlayerInventory playerInventory;

        [HideInInspector] public bool returnToBonfire = false;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            cursorManager = FindObjectOfType<CursorManager>(true);

            this.gameObject.SetActive(false);
        }

        void Translate(VisualElement root)
        {
            root.Q<Button>("ButtonExit").text = ButtonExit.GetText();
            root.Q<Label>("AvailableRecipes").text = AvailableRecipes.GetText();
            root.Q<Label>("RequiredIngredients").text = requiredIngredients.GetText();
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            BGMManager.instance.PlaySound(sfxOnEnterMenu, null);
            cursorManager.ShowCursor();

            Translate(root);

            DrawUI();
        }

        private void Update()
        {
            if (UnityEngine.Cursor.visible == false)
            {
                cursorManager.ShowCursor();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Gamepad.current != null && Gamepad.current.buttonEast.IsPressed())
            {
                Close();
            }
        }

        private void OnDisable()
        {
            cursorManager.HideCursor();
        }

        void Close()
        {
            if (returnToBonfire)
            {
                FindAnyObjectByType<UIDocumentBonfireMenu>(FindObjectsInactive.Include).gameObject.SetActive(true);
                returnToBonfire = false;
                cursorManager.ShowCursor();
                returnToBonfire = false;
                this.gameObject.SetActive(false);
                return;
            }

            FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
            FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
            this.gameObject.SetActive(false);
            cursorManager.HideCursor();
        }

        void DrawUI()
        {

            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            var buttonExit = root.Q<Button>("ButtonExit");
            uiManager.SetupButton(buttonExit, () =>
            {
                Close();
            });

            var craftActivityTitle = root.Q<Label>("CraftActivityTitle");

            if (craftActivity == CraftActivity.ALCHEMY)
            {
                root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(alchemyBackgroundImage);

                craftActivityTitle.text = AlchemyActivityTitle.GetText();
                PopulateScrollView(Player.instance.alchemyRecipes.ToArray());
            }
            else if (craftActivity == CraftActivity.COOKING)
            {
                root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(cookingBackgroundImage);

                craftActivityTitle.text = CookingActivityTitle.GetText();
                PopulateScrollView(Player.instance.cookingRecipes.ToArray());
            }
        }

        void PopulateScrollView(CraftingRecipe[] ownedCraftingRecipes)
        {
            var scrollView = this.root.Q<ScrollView>();
            scrollView.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
                FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
                this.gameObject.SetActive(false);
            });
            scrollView.Clear();

            if (ownedCraftingRecipes.Length <= 0) { return; }

            foreach (var recipe in ownedCraftingRecipes)
            {
                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(recipe.resultingItem.sprite);
                scrollItem.Q<Label>("ItemName").text = recipe.resultingItem.name.GetText();
                scrollItem.Q<Label>("ItemDescription").text = recipe.resultingItem.shortDescription.localizedTexts.Length > 0 ? recipe.resultingItem.shortDescription.GetText() : "";

                var craftBtn = scrollItem.Q<Button>("CraftButton");
                craftBtn.text = CraftButton.GetText();

                if (CanCraftItem(recipe))
                {
                    craftBtn.style.opacity = 1f;
                }
                else
                {
                    craftBtn.style.opacity = 0.25f;
                }

                uiManager.SetupButton(craftBtn, () =>
                {
                    if (craftBtn.enabledSelf == false)
                    {
                        return;
                    }

                    if (!CanCraftItem(recipe))
                    {
                        Soundbank.instance.PlayCraftError();
                        notificationManager.ShowNotification(missingIngredientsMessage.GetText(), notificationManager.alchemyLackOfIngredients);
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
                        notificationManager.ShowNotification(failedMixtureMessage.GetText(), notificationManager.alchemyLackOfIngredients);
                    }
                    else
                    {
                        if (craftActivity == CraftActivity.COOKING)
                        {
                            playerInventory.playerAchievementsManager.achievementForCookingFirstMeal.AwardAchievement();
                        }
                        else if (craftActivity == CraftActivity.ALCHEMY)
                        {
                            playerInventory.playerAchievementsManager.achievementForBrewingFirstPotion.AwardAchievement();
                        }


                        Soundbank.instance.PlayCraftSuccess();
                        playerInventory.AddItem(recipe.resultingItem, 1);
                        notificationManager.ShowNotification(receivedMessage.GetText() + " " + recipe.resultingItem.name.GetText(), recipe.resultingItem.sprite);
                    }


                    foreach (var ingredient in recipe.ingredients)
                    {
                        playerInventory.RemoveItem(ingredient.ingredient, ingredient.amount);
                    }

                    DrawUI();
                });

                craftBtn.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowRequiredIngredients(recipe);

                    scrollView.ScrollTo(craftBtn);
                });
                craftBtn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

                scrollItem.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowRequiredIngredients(recipe);
                });
                scrollItem.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

                scrollItem.RegisterCallback<MouseOverEvent>(ev =>
                {
                    ShowRequiredIngredients(recipe);
                });
                scrollItem.RegisterCallback<MouseOutEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

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

                var playerOwnedIngredient = Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == ingredient.ingredient.name.GetEnglishText());
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
                var itemEntry = Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == ingredient.ingredient.name.GetEnglishText());

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
    }
}
