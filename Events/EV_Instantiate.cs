using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class EV_Instantiate : EventBase
    {
        public GameObject objectToInstantiate;
        public Transform origin;
        public bool unparentOnInstantiate = true;

        public override IEnumerator Dispatch()
        {
            Instantiate(objectToInstantiate, origin.position, origin.transform.rotation, unparentOnInstantiate ? null : this.transform);
            yield return null;
        }
    }

}
