using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_LearnRecipe : EventBase
    {
        public CraftingRecipe recipe;
        public bool showNotificationText = true;

        public AudioClip pickUpSfx;

        public override IEnumerator Dispatch()
        {
            yield return LearnRecipe();
            
        }

        IEnumerator LearnRecipe()
        {
            Player.instance.LearnRecipe(recipe);

            if (showNotificationText)
            {
                Soundbank.instance.PlayItemReceived();

                var notf = FindObjectOfType<NotificationManager>(true);
                notf.ShowNotification("Learned recipe: " + recipe.name, notf.recipeIcon);
            }

            yield return null;
        }
    }

}
