using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ActivateLostCoinsPickup : MonoBehaviour
    {
        public int amount;

        public void ActivateLostCoinsPickupHandler()
        {
            LostCoinsManager.instance.CollectLostCoins(amount);
            this.gameObject.SetActive(false);
        }
    }

}
