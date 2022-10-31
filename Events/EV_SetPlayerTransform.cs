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
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<CharacterController>().enabled = false;
                player.transform.position = transformTarget.position;
                player.transform.rotation = transformTarget.rotation;
                player.GetComponent<CharacterController>().enabled = true;
            }

            yield return null;
        }
    }
}
