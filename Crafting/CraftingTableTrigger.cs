using UnityEngine;
using StarterAssets;

namespace AF
{
    public class CraftingTableTrigger : MonoBehaviour, IEventNavigatorCapturable
    {
        public UIDocumentAlchemyCraftScreen.CraftActivity craftActivity;

        StarterAssetsInputs inputs => FindObjectOfType<StarterAssetsInputs>(true);
        UIDocumentKeyPrompt uIDocumentKeyPrompt => FindObjectOfType<UIDocumentKeyPrompt>(true);
        UIDocumentAlchemyCraftScreen alchemyCraftScreen => FindObjectOfType<UIDocumentAlchemyCraftScreen>(true);

        public void OnCaptured()
        {
            uIDocumentKeyPrompt.key = "E";

            if (craftActivity == UIDocumentAlchemyCraftScreen.CraftActivity.ALCHEMY)
            {
                uIDocumentKeyPrompt.action = LocalizedTerms.UseAlchemyTable();
            }
            else if (craftActivity == UIDocumentAlchemyCraftScreen.CraftActivity.COOKING)
            {
                uIDocumentKeyPrompt.action = LocalizedTerms.Cook();
            }

            uIDocumentKeyPrompt.gameObject.SetActive(true);
        }

        public void OnInvoked()
        {
            uIDocumentKeyPrompt.gameObject.SetActive(false);

            FindObjectOfType<PlayerComponentManager>(true).DisableComponents();
            FindObjectOfType<PlayerComponentManager>(true).DisableCharacterController();

            alchemyCraftScreen.craftActivity = craftActivity;
            alchemyCraftScreen.gameObject.SetActive(true);
        }
    }
}
