using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ManageGameObject : EventBase
    {
        public GameObject gameObjectTarget;

        public bool isActive;

        public override IEnumerator Dispatch()
        {    
            gameObjectTarget.SetActive(isActive);
            yield return null;
        }
    }

}
