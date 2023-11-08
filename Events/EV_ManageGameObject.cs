using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ManageGameObject : EventBase
    {
        public GameObject gameObjectTarget;

        Vector3 position;
        Quaternion rotation;


        public bool isActive;

        private void Awake()
        {
            position = gameObject.transform.position;
            rotation = gameObject.transform.rotation;  
        }

        public override IEnumerator Dispatch()
        {    
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;  
            gameObjectTarget.SetActive(isActive);
            yield return null;
        }
    }

}
