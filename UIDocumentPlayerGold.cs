using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentPlayerGold : MonoBehaviour
    {
        UIDocument uiDocument => GetComponent<UIDocument>();

        VisualElement root;

        int currentReceivedAmount = 0;
        int playerGold = 0;

        bool counterEnabled = false;

        public float scoreIncreaseRate = 1.25f;
        float ScoreIncrement = 0;

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void NotifyGold(int amount)
        {
            BGMManager.instance.PlayCoin();
            counterEnabled = false;
            ScoreIncrement = 0;

            StopAllCoroutines();

            this.currentReceivedAmount = amount;
            this.playerGold = Player.instance.currentGold;

            this.gameObject.SetActive(true);

            this.root = uiDocument.rootVisualElement;

            root.Q<Label>("GoldReceived").text = "+ " + currentReceivedAmount;
            root.Q<Label>("ActualGold").text = "" + playerGold + " Gold";

            StartCoroutine(EnableCounter());
        }

        IEnumerator EnableCounter()
        {
            yield return new WaitForSeconds(1f);
            counterEnabled = true;
            Debug.Log("activate counterEnabled");
        }


        private void Update()
        {
            Debug.Log("counterEnabled" + counterEnabled);
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
                root.Q<Label>("ActualGold").text = "" + playerGold + " Gold";
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
            yield return new WaitForSeconds(3f);
            this.gameObject.SetActive(false);
        }


    }

}
