using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentAlchemyCraftScreen : MonoBehaviour
    {
        public enum CraftActivity
        {
            ALCHEMY,
            COOKING,
        }

        public CraftActivity craftActivity;

        [Header("UI")]
        public VisualTreeAsset recipeItem;
        public VisualTreeAsset ingredientItem;
        public Sprite alchemyBackgroundImage;
        public Sprite cookingBackgroundImage;

        UIDocument uIDocument => GetComponent<UIDocument>();
        [HideInInspector] public VisualElement root;

        PlayerInventory playerInventory;

        MenuManager menuManager;

        public AudioClip sfxOnEnterMenu;

        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
            menuManager = FindObjectOfType<MenuManager>(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            playerInventory = FindObjectOfType<PlayerInventory>(true);


            Utils.ShowCursor();

            BGMManager.instance.PlaySound(sfxOnEnterMenu, null);


            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);

            DrawUI();
        }

        private void OnDisable()
        {
            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
        }

        void DrawUI()
        {

            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            var buttonExit = root.Q<Button>("ButtonExit");
            menuManager.SetupButton(buttonExit, () =>
            {
                FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
                FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
                this.gameObject.SetActive(false);
                Utils.HideCursor();
            });

            var craftActivityTitle = root.Q<Label>("CraftActivityTitle");

            if (craftActivity == CraftActivity.ALCHEMY)
            {
                root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(alchemyBackgroundImage);

                buttonExit.text = "Exit Alchemy";
                craftActivityTitle.text = "Alchemy Table";
                PopulateScrollView(Player.instance.alchemyRecipes.ToArray());
            }
            else if (craftActivity == CraftActivity.COOKING)
            {
                root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(cookingBackgroundImage);

                buttonExit.text = "Exit Cooking";
                craftActivityTitle.text = "Cooking Table";
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
                scrollItem.Q<Label>("ItemDescription").text = recipe.resultingItem.description.GetText();

                var craftBtn = scrollItem.Q<Button>("CraftButton");
                
                if (CanCraftItem(recipe))
                {
                    craftBtn.style.opacity = 1f;
                }
                else
                {
                    craftBtn.style.opacity = 0.25f;
                }

                menuManager.SetupButton(craftBtn, () =>
                {
                    if (!CanCraftItem(recipe))
                    {
                        Soundbank.instance.PlayCraftError();
                        notificationManager.ShowNotification("Crafting failed: missing ingredients", notificationManager.alchemyLackOfIngredients);
                        return;
                    }

                    Soundbank.instance.PlayCraftSuccess();

                    playerInventory.AddItem(recipe.resultingItem, 1);

                    notificationManager.ShowNotification("Received " + recipe.resultingItem.name, recipe.resultingItem.sprite);

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

                var playerOwnedIngredient = Player.instance.ownedItems.Find(x => x.item == ingredient.ingredient);
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
                var itemEntry = Player.instance.ownedItems.Find(x => x.item == ingredient.ingredient);

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