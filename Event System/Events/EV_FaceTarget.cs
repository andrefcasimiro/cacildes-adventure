using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_FaceTarget : EventBase
    {
        public Transform character;
        public Transform lookTarget;

        public bool facePlayer = false;

        public override IEnumerator Dispatch()
        {
            if (facePlayer)
            {
                lookTarget = FindObjectOfType<Player>().transform;
            }

            var lookPos = lookTarget.transform.position - character.transform.position;
            lookPos.y = 0;
            character.transform.rotation = Quaternion.LookRotation(lookPos);

            yield return null;
        }
    }
}