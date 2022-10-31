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
            yield return null;

            gameObjectTarget.gameObject.SetActive(isActive);
        }
    }

}
