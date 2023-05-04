using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBlacksmithScreen : MonoBehaviour
    {

        [Header("UI")]
        public VisualTreeAsset recipeItem;
        public VisualTreeAsset ingredientItem;
        public Sprite backgroundImage;

        public AudioClip sfxOnEnterMenu;

        public Sprite goldSprite;

        [Header("Localization")]
        public LocalizedText ButtonExit;
        public LocalizedText CraftButton;
        public LocalizedText missingIngredientsMessage;
        public LocalizedText receivedMessage;

        UIDocument uIDocument => GetComponent<UIDocument>();
        [HideInInspector] public VisualElement root;

        NotificationManager notificationManager;
        MenuManager menuManager;
        PlayerInventory playerInventory;

        private void Awake()
        {
             notificationManager = FindObjectOfType<NotificationManager>(true);
             menuManager = FindObjectOfType<MenuManager>(true);
             playerInventory = FindObjectOfType<PlayerInventory>(true);

            this.gameObject.SetActive(false);
        }

        void Translate(VisualElement root)
        {
            root.Q<Button>("ButtonExit").text = ButtonExit.GetText();
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            BGMManager.instance.PlaySound(sfxOnEnterMenu, null);
            Utils.ShowCursor();

            Translate(root);

            DrawUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Gamepad.current != null && Gamepad.current.buttonEast.IsPressed())
            {
                Close();
            }
        }

        private void OnDisable()
        {
            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
        }

        void Close()
        {

            FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
            FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
            this.gameObject.SetActive(false);
            Utils.HideCursor();
        }

        void DrawUI()
        {

            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            var buttonExit = root.Q<Button>("ButtonExit");
            menuManager.SetupButton(buttonExit, () =>
            {
                Close();
            });

            var craftActivityTitle = root.Q<Label>("CraftActivityTitle");
            craftActivityTitle.text = LocalizedTerms.AnvilTable();
            root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(backgroundImage);


            // Clear Weapon preview
            root.Q<Label>("WeaponLevelPreview").text = "";
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;

            PopulateScrollView();

            /*
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
            }*/
        }

        void PopulateScrollView()
        {
            var scrollView = this.root.Q<ScrollView>();
            scrollView.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
                FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
                this.gameObject.SetActive(false);
            });
            scrollView.Clear();

            List<Player.ItemEntry> weapons = new();

            foreach (var it in Player.instance.ownedItems)
            {
                if ((it.item as Weapon) != null)
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
                craftBtn.text = CraftButton.GetText();

                if (CanImproveWeapon(wp))
                {
                    craftBtn.style.opacity = 1f;
                }
                else
                {
                    craftBtn.style.opacity = 0.25f;
                }

                menuManager.SetupButton(craftBtn, () =>
                {
                    if (!CanImproveWeapon(wp))
                    {
                        Soundbank.instance.PlayCraftError();
                        notificationManager.ShowNotification(missingIngredientsMessage.GetText(), notificationManager.alchemyLackOfIngredients);
                        return;
                    }

                    Soundbank.instance.PlayCraftSuccess();

                    notificationManager.ShowNotification(receivedMessage.GetText(), wp.sprite);

                    var currentWeaponLevel = wp.level;
                    Player.instance.currentGold -= wp.GetRequiredUpgradeGoldForGivenLevel(currentWeaponLevel + 1);
                    playerInventory.RemoveItem(wp.upgradeMaterial, wp.GetRequiredOresForGivenLevel(currentWeaponLevel + 1));

                    wp.level++;


                    DrawUI();
                });

                craftBtn.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowRequirements(wp);

                    scrollView.ScrollTo(craftBtn);
                });
                craftBtn.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

                scrollItem.RegisterCallback<FocusInEvent>(ev =>
                {
                    ShowRequirements(wp);
                });
                scrollItem.RegisterCallback<FocusOutEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

                scrollItem.RegisterCallback<MouseOverEvent>(ev =>
                {
                    ShowRequirements(wp);
                });
                scrollItem.RegisterCallback<MouseOutEvent>(ev =>
                {
                    root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;
                });

                scrollView.Add(scrollItem);
            }

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

            var playerOwnedIngredient = Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == weapon.upgradeMaterial.name.GetEnglishText());
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

            goldItemEntry.Q<Label>("Amount").text = Player.instance.currentGold + " / " + weapon.GetRequiredUpgradeGoldForGivenLevel(nextLevel);
            goldItemEntry.Q<Label>("Amount").style.opacity = Player.instance.currentGold >= weapon.GetRequiredUpgradeGoldForGivenLevel(nextLevel) ? 1 : 0.25f;

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

            var itemEntry = Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == weapon.upgradeMaterial.name.GetEnglishText());

            if (itemEntry == null)
            {
                return false;
            }

            if (itemEntry.amount < weapon.GetRequiredOresForGivenLevel(nextLevel))
            {
                return false;
            }

            if (Player.instance.currentGold < weapon.GetRequiredUpgradeGoldForGivenLevel(nextLevel))
            {
                return false;
            }

            return true;
        }
    }
}
