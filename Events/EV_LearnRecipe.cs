using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class EV_LearnRecipe : EventBase
    {
        public CraftingRecipe recipe;
        public bool showNotificationText = true;

        [Header("Components")]
        Soundbank _soundbank;
        NotificationManager _notificationManager;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;

        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

        NotificationManager GetNotificationManager()
        {
            if (_notificationManager == null)
            {
                _notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }

            return _notificationManager;
        }

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
                GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);

                GetNotificationManager().ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Learned recipe:") + " "
                     + recipe.resultingItem.GetName(), GetNotificationManager().recipeIcon);
            }

            yield return null;
        }
    }
}
