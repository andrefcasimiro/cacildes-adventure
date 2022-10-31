using System.Collections;
using System.Collections.Generic;
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
        public Sprite backgroundImage;

        UIDocument uIDocument => GetComponent<UIDocument>();
        [HideInInspector] public VisualElement root;

        PlayerInventory playerInventory;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            playerInventory = FindObjectOfType<PlayerInventory>(true);

            UnityEngine.Cursor.lockState = CursorLockMode.None;

            DrawUI();
        }

        private void OnDisable()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }

        void DrawUI()
        {
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            var buttonExit = root.Q<Button>("ButtonExit");
            buttonExit.RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
                FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
                this.gameObject.SetActive(false);
            });

            var craftActivityTitle = root.Q<Label>("CraftActivityTitle");

            root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(backgroundImage);

            if (craftActivity == CraftActivity.ALCHEMY)
            {
                buttonExit.text = "Exit Alchemy";
                craftActivityTitle.text = "Alchemy Table";
                var alchemyRecipes = Player.instance.alchemyRecipes;
                    PopulateScrollView(alchemyRecipes);
            }
            else if (craftActivity == CraftActivity.COOKING)
            {
                buttonExit.text = "Exit Cooking";
                craftActivityTitle.text = "Cooking Table";
            }
        }

        void PopulateScrollView(List<AlchemyRecipe> ownedCraftingRecipes)
        {
            var scrollView = this.root.Q<ScrollView>();
            scrollView.Clear();

            if (ownedCraftingRecipes.Count <= 0) { return; }

            foreach (var recipe in ownedCraftingRecipes)
            {
                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(recipe.resultingItem.sprite);
                scrollItem.Q<Label>("ItemName").text = (recipe.resultingItem.name);
                scrollItem.Q<Label>("ItemDescription").text = (recipe.resultingItem.description);

                scrollItem.Q<Button>("CraftButton").SetEnabled(CanCraftAlchemyItem(recipe));

                scrollItem.Q<Button>("CraftButton").RegisterCallback<ClickEvent>(ev =>
                {
                    playerInventory.AddItem(recipe.resultingItem, 1);

                    foreach (var ingredient in recipe.ingredients)
                    {
                        playerInventory.RemoveItem(ingredient.ingredient, ingredient.amount);
                    }

                    DrawUI();
                });

                scrollItem.RegisterCallback<PointerEnterEvent>(ev =>
                {
                    root.Q<VisualElement>("ItemInfo").Clear();

                    foreach (var ingredient in recipe.ingredients)
                    {
                        var ingredientItemEntry = ingredientItem.CloneTree();
                        ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(ingredient.ingredient.sprite);
                        ingredientItemEntry.Q<Label>("Title").text = ingredient.ingredient.name;

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
                });
                scrollItem.RegisterCallback<PointerLeaveEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

                scrollView.Add(scrollItem);
            }
        }

        bool CanCraftAlchemyItem(AlchemyRecipe recipe)
        {
            bool hasEnoughMaterial = true;

            foreach (var ingredient in recipe.ingredients)
            {
                var itemEntry = Player.instance.ownedItems.Find(x => x.item == ingredient.ingredient);

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