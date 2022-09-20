using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EV_SetPlayerTransform : EventBase
    {
        public GameObject target;
        public Transform transformTarget;

        public override IEnumerator Dispatch()
        {
            if (target != null)
            {
                target.transform.position = transformTarget.position;
                target.transform.rotation = transformTarget.rotation;
            }
            else
            {

                FindObjectOfType<Player>(true).transform.position = transformTarget.position;
                FindObjectOfType<Player>(true).transform.rotation = transformTarget.rotation;

            }

            yield return null;
        }
    }
}
