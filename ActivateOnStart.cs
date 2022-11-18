using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ActivateOnStart : MonoBehaviour
    {
        private void Awake()
        {
            this.gameObject.SetActive(true);
        }
    }

}