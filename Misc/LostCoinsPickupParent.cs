using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class LostCoinsPickupParent : MonoBehaviour, ISaveable
    {

        private void Start()
        {
            LostCoinsManager.instance.ActivateLostCoinsPickupInMap();
        }

        public void OnGameLoaded(object gameData)
        {
            LostCoinsManager.instance.ActivateLostCoinsPickupInMap();
        }
    }

}
