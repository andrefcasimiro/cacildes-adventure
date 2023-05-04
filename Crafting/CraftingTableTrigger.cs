using UnityEngine;
using StarterAssets;

namespace AF
{
    public class CraftingTableTrigger : MonoBehaviour, IEventNavigatorCapturable
    {
        public UIDocumentAlchemyCraftScreen.CraftActivity craftActivity;

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        UIDocumentAlchemyCraftScreen alchemyCraftScreen;
        UIDocumentBlacksmithScreen blacksmithScreen;

        private void Awake()
        {

             inputs = FindObjectOfType<StarterAssetsInputs>(true);
             uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
             alchemyCraftScreen = FindObjectOfType<UIDocumentAlchemyCraftScreen>(true);
             blacksmithScreen = FindObjectOfType<UIDocumentBlacksmithScreen>(true);

        }

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
            else if (craftActivity == UIDocumentAlchemyCraftScreen.CraftActivity.BLACKSMITH)
            {
                uIDocumentKeyPrompt.action = LocalizedTerms.UseBlacksmithAnvil();
            }

            uIDocumentKeyPrompt.gameObject.SetActive(true);
        }

        public void OnInvoked()
        {
            uIDocumentKeyPrompt.gameObject.SetActive(false);

            FindObjectOfType<PlayerComponentManager>(true).DisableComponents();
            FindObjectOfType<PlayerComponentManager>(true).DisableCharacterController();

            if (craftActivity == UIDocumentAlchemyCraftScreen.CraftActivity.BLACKSMITH)
            {
                blacksmithScreen.gameObject.SetActive(true);
                return;
            }

            alchemyCraftScreen.craftActivity = craftActivity;
            alchemyCraftScreen.gameObject.SetActive(true);
        }
    }
}
