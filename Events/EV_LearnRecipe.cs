using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_LearnRecipe : EventBase
    {
        public CraftingRecipe recipe;
        public bool showNotificationText = true;
        public AudioClip pickUpSfx;

        [Header("Components")]
        public Soundbank soundbank;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;

        public override IEnumerator Dispatch()
        {
            yield return _LearnRecipe();
        }

        IEnumerator _LearnRecipe()
        {
            LearnRecipe();

            if (showNotificationText)
            {
                soundbank.PlaySound(soundbank.uiItemReceived);

                var notf = FindObjectOfType<NotificationManager>(true);
                notf.ShowNotification("Learned recipe: " + recipe.name.GetEnglishText(), notf.recipeIcon);
            }

            yield return null;
        }

        public void LearnRecipe()
        {
            if (
                recipesDatabase.cookingRecipes.Contains(recipe as CookingRecipe)
                || recipesDatabase.alchemyRecipes.Contains(recipe as AlchemyRecipe))
            {
                return;
            }

            var cookingRecipe = (CookingRecipe)recipe;
            if (cookingRecipe != null)
            {
                recipesDatabase.cookingRecipes.Add(cookingRecipe);
                return;
            }

            var alchemyRecipe = (AlchemyRecipe)recipe;
            if (alchemyRecipe != null)
            {
                recipesDatabase.alchemyRecipes.Add(alchemyRecipe);
                return;
            }
        }
    }
}
