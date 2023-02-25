using UnityEngine;
using StarterAssets;
using UnityEngine.SceneManagement;

namespace AF
{
    public class ItemPickup : SwitchListener
    {
        public Item item;

        public Item[] items;

        public AlchemyRecipe alchemyRecipe;

        public int quantity = 1;
        public int gold = -1;

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;

        [Header("Notification")]
        public string actionName = "Pick up Item";

        [Header("Analytics")]
        public bool trackPickup = false;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
            uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        private void Start()
        {
            Refresh();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = actionName;
            uIDocumentKeyPrompt.gameObject.SetActive(true);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            if (inputs.interact)
            {
                inputs.interact = false;

                uIDocumentKeyPrompt.gameObject.SetActive(false);

                if (gold != -1)
                {
                    FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(gold);
                    Player.instance.currentGold += gold;

                    if (trackPickup)
                    {
                        /*AnalyticsService.Instance.CustomData("gold_found",
                                new Dictionary<string, object>()
                                {
                                    { "amount", gold },
                                    { "scene", SceneManager.GetActiveScene() }
                                }
                            );*/
                    }
                }

                // Is Alchemy Recipe?
                else if (alchemyRecipe != null)
                {
                    Player.instance.alchemyRecipes.Add(alchemyRecipe);
                }
                else if (item != null) // Normal Item
                {
                    UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                    itemReceived.itemName = item.name.GetText();
                    itemReceived.quantity = quantity;
                    itemReceived.sprite = item.sprite;

                    uIDocumentReceivedItemPrompt.itemsUI.Clear();
                    uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);

                    uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
                    playerInventory.AddItem(item, quantity);

                    if (trackPickup)
                    {
                        /*AnalyticsService.Instance.CustomData("item_found",
                                new Dictionary<string, object>()
                                {
                                    { "item_name", item.name },
                                    { "quantity", quantity },
                                    { "scene", SceneManager.GetActiveScene().name }
                                }
                            );*/
                    }

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

                        if (trackPickup)
                        {
                            /*AnalyticsService.Instance.CustomData("item_found",
                                    new Dictionary<string, object>()
                                    {
                                    { "item_name", item.name },
                                    { "quantity", 1 },
                                    { "scene", SceneManager.GetActiveScene().name }
                                    }
                                );*/
                        }
                    }

                    uIDocumentReceivedItemPrompt.gameObject.SetActive(true);

                    Soundbank.instance.PlayItemReceived();
                }

                if (switchEntry == null)
                {
                    return;
                }

                SwitchManager.instance.UpdateSwitch(switchEntry, true);

                if (gold != -1)
                {
                    // Save Game to prevent us from dying and picking money infinitely
                    SaveSystem.instance.currentScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
                    SaveSystem.instance.SaveGameData(SceneManager.GetActiveScene().name);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (uIDocumentKeyPrompt == null)
            {
                return;
            }

            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }

        public override void Refresh()
        {
            gameObject.SetActive(!SwitchManager.instance.GetSwitchCurrentValue(switchEntry));
        }
    }
}
