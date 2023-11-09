using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class Fogwall : MonoBehaviour, ISaveable
    {
        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        public void OnGameLoaded(object gameData)
        {
            this.gameObject.SetActive(false);
        }
    }

}