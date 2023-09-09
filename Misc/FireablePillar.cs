using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class FireablePillar : MonoBehaviour
    {
        public UnityEvent onRecievingFire;
        bool hasInvokedEvent = false;

        public void Explode()
        {
            if (hasInvokedEvent == false)
            {
                hasInvokedEvent = true;
                onRecievingFire.Invoke();
            }
            this.enabled = false;
        }

    }

}
