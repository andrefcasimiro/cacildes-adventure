using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ActionPushCarriage : MonoBehaviour
    {
        public bool isRunning = false;

        public MinecartCollider minecartCollider;

        public void PushCarriage()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;
            minecartCollider.ActivateCart();
        }
    }

}
