using UnityEngine;
using StarterAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace AF
{
    public class ItemPickup : SwitchListener, IEventNavigatorCapturable
    {
        public Item item;

        public Item[] items;

        public AlchemyRecipe alchemyRecipe;

        public int quantity = 1;
        public int gold = -1;

        public UnityEvent onPickupEvent;

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;

        [Header("Notification")]
        public LocalizedTerms.LocalizedAction actionName = LocalizedTerms.LocalizedAction.PICKUP_ITEM;

        [Header("Analytics")]
        public string analyticsMessage;

        private void Awake()
        {
             inputs = FindObjectOfType<StarterAssetsInputs>(true);
             uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
             uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);
             playerInventory = FindObjectOfType<PlayerInventory>(true);
        }

        private void Start()
        {
            Refresh();
        }

        public override void Refresh()
        {
            gameObject.SetActive(!SwitchManager.instance.GetSwitchCurrentValue(switchEntry));
        }

        public void OnCaptured()
        {
            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = LocalizedTerms.PickupItem();
            uIDocumentKeyPrompt.gameObject.SetActive(true);
        }

        public void OnInvoked()
        {
            inputs.interact = false;

            uIDocumentKeyPrompt.gameObject.SetActive(false);

            if (onPickupEvent != null)
            {
                onPickupEvent.Invoke();
            }

            if (gold != -1)
            {
                FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(gold);
                Player.instance.currentGold += gold;

            }

            // Is Alchemy Recipe?
            else if (alchemyRecipe != null)
            {
                Player.instance.alchemyRecipes.Add(alchemyRecipe);
            }
            else if (item != null) // Normal Item
            {
                playerInventory.AddItem(item, quantity);

                UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                itemReceived.itemName = item.name.GetText();
                itemReceived.quantity = quantity;
                itemReceived.sprite = item.sprite;

                uIDocumentReceivedItemPrompt.itemsUI.Clear();
                uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);


                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);

                Soundbank.instance.PlayItemReceived();
            }
            else if (items.Length > 0)
            {
                uIDocumentReceivedItemPrompt.itemsUI.Clear();

                foreach (var item in items)
                {
                    UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                    itemReceived.itemName = item.name.GetText();
                    itemReceived.quantity = 1;
                    itemReceived.sprite = item.sprite;

                    uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);

                    playerInventory.AddItem(item, 1);


                }

                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);

                Soundbank.instance.PlayItemReceived();
            }

            if (switchEntry == null)
            {
                return;
            }

            SwitchManager.instance.UpdateSwitch(switchEntry, true, null);

            if (gold != -1)
            {
                // Save Game to prevent us from dying and picking money infinitely
                SaveSystem.instance.currentScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
                SaveSystem.instance.SaveGameData(SceneManager.GetActiveScene().name);
            }


            if (!string.IsNullOrEmpty(analyticsMessage))
            {
                FindObjectOfType<Analytics>(true).TrackAnalyticsEvent(analyticsMessage);
            }

        }
    }
}
