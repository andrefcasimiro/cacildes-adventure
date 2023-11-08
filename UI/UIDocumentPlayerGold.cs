using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentPlayerGold : MonoBehaviour
    {
        UIDocument uiDocument => GetComponent<UIDocument>();

        VisualElement root;

        int currentReceivedAmount = 0;
        int currentDeductedAmount = 0;
        int playerGold = 0;

        bool counterEnabled = false;

        public float scoreIncreaseRate = 1.25f;
        float ScoreIncrement = 0;

        [HideInInspector] public ParticlePoolManager particlePoolManager;

        ParticleSystem currentGold;

        private void Awake()
        {
            particlePoolManager = FindAnyObjectByType<ParticlePoolManager>(FindObjectsInactive.Include);

            this.gameObject.SetActive(false);
        }

        #region Positive Gold
        public void PlayCoinsFX(Transform origin)
        {
            if (currentGold != null)
            {
                currentGold.Stop();
                particlePoolManager.goldPool.Pool.Release(currentGold);
            }

            currentGold = particlePoolManager.goldPool.Pool.Get();

            
            if (currentGold == null)
            {
                return;
            }

            currentGold.transform.position = origin.position;
            currentGold.Play();
            var m = currentGold.main;
            m.loop = false;
        }

        public void NotifyGold(int amount)
        {
            Soundbank.instance.PlayCoin();
            counterEnabled = false;
            ScoreIncrement = 0;

            StopAllCoroutines();

            this.currentReceivedAmount = amount;
            this.playerGold = Player.instance.currentGold;

            this.gameObject.SetActive(true);

            this.root = uiDocument.rootVisualElement;

            root.Q<Label>("GoldReceived").text = "+ " + currentReceivedAmount;
            root.Q<Label>("ActualGold").text = "" + playerGold + " " + LocalizedTerms.Gold();

            StartCoroutine(EnableCounter());
        }

        IEnumerator EnableCounter()
        {
            yield return new WaitForSeconds(1f);
            counterEnabled = true;
        }
        #endregion

        #region Negative Gold
        public void NotifyGoldLost(int amount)
        {
            Soundbank.instance.PlayCoin();
            counterEnabled = false;
            ScoreIncrement = 0;

            StopAllCoroutines();

            this.currentDeductedAmount = amount;
            this.playerGold = Player.instance.currentGold;

            this.gameObject.SetActive(true);

            this.root = uiDocument.rootVisualElement;

            root.Q<Label>("GoldReceived").text = "- " + currentDeductedAmount;
            root.Q<Label>("ActualGold").text = "" + playerGold + " " + LocalizedTerms.Gold();

            StartCoroutine(EnableCounter());
        }
        #endregion

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
                if (currentReceivedAmount < 0) { currentReceivedAmount = 0; }

                playerGold += (int)ScoreIncrement;

                if (playerGold > Player.instance.currentGold) { playerGold = Player.instance.currentGold; }


                root.Q<Label>("GoldReceived").text = "+ " + currentReceivedAmount;
                root.Q<Label>("ActualGold").text = "" + playerGold + " " + LocalizedTerms.Gold();
            }
            else if (currentDeductedAmount > 0)
            {
                ScoreIncrement += Time.deltaTime * scoreIncreaseRate;

                currentDeductedAmount -= (int)ScoreIncrement;
                if (currentDeductedAmount < 0) { currentDeductedAmount = 0; }

                playerGold -= (int)ScoreIncrement;

                if (playerGold <= Player.instance.currentGold) { playerGold = Player.instance.currentGold; }


                root.Q<Label>("GoldReceived").text = "- " + currentDeductedAmount;
                root.Q<Label>("ActualGold").text = "" + playerGold + " " + LocalizedTerms.Gold();
            }
            else
            {
                counterEnabled = false;
                root.Q<Label>("GoldReceived").text = "";
                StartCoroutine(Exit());
            }
        }

        IEnumerator Exit()
        {
            if (currentGold != null)
            {
                currentGold.Stop();
            }

            yield return new WaitForSeconds(3f);

            if (currentGold != null)
            {

                particlePoolManager.goldPool.Pool.Release(currentGold);
                currentGold = null;
            }

            this.gameObject.SetActive(false);
        }
    }
}
