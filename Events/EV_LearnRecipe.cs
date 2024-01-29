using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_LearnRecipe : EventBase
    {
        public CraftingRecipe recipe;
        public bool showNotificationText = true;

        [Header("Components")]
        public Soundbank soundbank;
        public NotificationManager notificationManager;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;

        public override IEnumerator Dispatch()
        {
            yield return _LearnRecipe();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void LearnRecipe()
        {
            StartCoroutine(Dispatch());
        }

        IEnumerator _LearnRecipe()
        {
            if (recipesDatabase.HasRecipe(recipe))
            {
                yield break;
            }

            recipesDatabase.AddCraftingRecipe(recipe);

            if (showNotificationText)
            {
                soundbank.PlaySound(soundbank.uiItemReceived);

                notificationManager.ShowNotification("Learned recipe: " + recipe.name, notificationManager.recipeIcon);
            }

            yield return null;
        }
    }
}
