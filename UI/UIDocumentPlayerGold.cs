using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentPlayerGold : MonoBehaviour
    {
        [Header("Components")]
        public Soundbank soundbank;

        [Header("UI Components")]
        public UIDocument uiDocument;

        VisualElement root;

        int currentReceivedAmount, currentDeductedAmount, playerGold = 0;

        bool counterEnabled = false;

        float ScoreIncrement = 0;

        [Header("Settings")]
        public float scoreIncreaseRate = 1.25f;

        [Header("Stats Database")]
        public GameSession gameSession;
        public PlayerStatsDatabase playerStatsDatabase;

        Label goldReceived, actualGold;

        [Header("Localization")]
        public LocalizedString gold_LocalizedString;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uiDocument.rootVisualElement;

            goldReceived = root.Q<Label>("GoldReceived");
            actualGold = root.Q<Label>("ActualGold");
        }

        public void AddGold(int amount)
        {
            amount = Utils.ScaleWithCurrentNewGameIteration(amount, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);

            soundbank.PlaySound(soundbank.coin);
            counterEnabled = false;
            ScoreIncrement = 0;

            StopAllCoroutines();

            this.currentReceivedAmount = amount;
            this.playerGold = playerStatsDatabase.gold;

            this.gameObject.SetActive(true);

            UIUtils.PlayPopAnimation(goldReceived);

            goldReceived.text = "+ " + currentReceivedAmount;
            actualGold.text = "" + playerGold + " " + gold_LocalizedString.GetLocalizedString();
            playerStatsDatabase.gold += amount;

            StartCoroutine(EnableCounter());
        }

        public void LoseGold(int amount)
        {
            soundbank.PlaySound(soundbank.coin);
            counterEnabled = false;
            ScoreIncrement = 0;

            StopAllCoroutines();

            this.currentDeductedAmount = amount;
            this.playerGold = playerStatsDatabase.gold;

            this.gameObject.SetActive(true);

            UIUtils.PlayPopAnimation(goldReceived);

            goldReceived.text = "- " + currentDeductedAmount;
            actualGold.text = "" + playerGold + " " + gold_LocalizedString.GetLocalizedString();

            playerStatsDatabase.gold -= amount;

            StartCoroutine(EnableCounter());
        }

        IEnumerator EnableCounter()
        {
            yield return new WaitForSeconds(1f);
            counterEnabled = true;
        }

        private void Update()
        {
            if (!counterEnabled)
            {
                return;
            }

            if (currentReceivedAmount > 0)
            {
                ScoreIncrement += Time.deltaTime * scoreIncreaseRate;

                currentReceivedAmount -= (int)ScoreIncrement;
                if (currentReceivedAmount < 0)
                {
                    currentReceivedAmount = 0;
                }

                playerGold += (int)ScoreIncrement;

                if (playerGold > playerStatsDatabase.gold)
                {
                    playerGold = playerStatsDatabase.gold;
                }

                goldReceived.text = "+ " + currentReceivedAmount;
                actualGold.text = "" + playerGold + " " + gold_LocalizedString.GetLocalizedString();
            }
            else if (currentDeductedAmount > 0)
            {
                ScoreIncrement += Time.deltaTime * scoreIncreaseRate;

                currentDeductedAmount -= (int)ScoreIncrement;
                if (currentDeductedAmount < 0)
                {
                    currentDeductedAmount = 0;
                }

                playerGold -= (int)ScoreIncrement;

                if (playerGold <= playerStatsDatabase.gold)
                {
                    playerGold = playerStatsDatabase.gold;
                }

                goldReceived.text = "- " + currentDeductedAmount;
                actualGold.text = "" + playerGold + " " + gold_LocalizedString.GetLocalizedString();
            }
            else
            {
                counterEnabled = false;
                goldReceived.text = "";
                this.gameObject.SetActive(false);
            }
        }
    }
}
