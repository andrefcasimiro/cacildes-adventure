using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AF
{
    public class LostCoinsManager : MonoBehaviour
    {
        public static LostCoinsManager instance;

        public LostCoins lostCoins;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        /// <summary>
        /// Called on game over event
        /// </summary>
        public void SetCoinsToRecover(Transform deathTransform)
        {
            if (this.lostCoins != null && this.lostCoins.amount > 0)
            {
                // Lose coins forever
                this.lostCoins = null;
                return;
            }

            LostCoins lostCoins = new LostCoins();

            lostCoins.amount = Player.instance.currentGold;

            Player.instance.currentGold = 0;

            NavMeshHit hit;
            NavMesh.SamplePosition(deathTransform.position, out hit, 10f, NavMesh.AllAreas);

            if (hit.position != null)
            {
                lostCoins.position = hit.position;
            }
            else
            {
                lostCoins.position = deathTransform.position;
            }

            lostCoins.sceneIndex = SceneManager.GetActiveScene().buildIndex;

            this.lostCoins = lostCoins;
        }

        public void ActivateLostCoinsPickupInMap()
        {
            if (lostCoins == null)
            {
                return;
            }

            var activateLostCoinsPickup = FindObjectOfType<ActivateLostCoinsPickup>(true);

            // If we are not in the map where the coins were lost, deactivate them
            if (lostCoins.sceneIndex != SceneManager.GetActiveScene().buildIndex)
            {
                activateLostCoinsPickup.gameObject.SetActive(false);
                return;
            }

            activateLostCoinsPickup.transform.position = lostCoins.position;
            activateLostCoinsPickup.amount = lostCoins.amount;
            activateLostCoinsPickup.gameObject.SetActive(true);
        }

        public void CollectLostCoins(int amount)
        {
            FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(amount);

            Player.instance.currentGold += amount;

            this.lostCoins = null;
        }


    }

}
