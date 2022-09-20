using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace AF
{

    public class UIDocumentCreditsScreen : UIDocumentBase
    {
        Button assetsButton;
        Button specialThanksButton;

        public bool showingAssets = true;

        protected override void Start()
        {
            base.Start();


             assetsButton = this.root.Q<Button>("AssetsButton");
             specialThanksButton = this.root.Q<Button>("SpecialThanksButton");
            Button goGackButton = this.root.Q<Button>("GoBackButton");
            
            SetupButtonClick(assetsButton, () =>
            {
                showingAssets = true;
                UpdateUI();
            });
            SetupButtonClick(specialThanksButton, () =>
            {
                showingAssets = false;
                UpdateUI();
            });
            assetsButton.Focus();

            SetupButtonClick(goGackButton, () =>
            {
                this.Disable();

                FindObjectOfType<UIDocumentTitleScreen>(true).Enable();

            });

            UpdateUI();


            this.Disable();
        }

        void UpdateUI()
        {
            if (showingAssets)
            {
                this.root.Q<VisualElement>("Assets").RemoveFromClassList("hide");
                this.root.Q<VisualElement>("Thanks").AddToClassList("hide");

                assetsButton.AddToClassList("credits-game-button-active");
                specialThanksButton.RemoveFromClassList("credits-game-button-active");
            }
            else
            {
                this.root.Q<VisualElement>("Thanks").RemoveFromClassList("hide");
                this.root.Q<VisualElement>("Assets").AddToClassList("hide");

                assetsButton.RemoveFromClassList("credits-game-button-active");
                specialThanksButton.AddToClassList("credits-game-button-active");
            }

        }


    }

}