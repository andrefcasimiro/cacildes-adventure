using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_ReleaseLockOn : EventBase
    {

        LockOnManager lockOnManager;

        public override IEnumerator Dispatch()
        {
            if (lockOnManager == null)
            {
                lockOnManager = FindAnyObjectByType<LockOnManager>(FindObjectsInactive.Include);
            }

            if (lockOnManager != null)
            {
                lockOnManager.DisableLockOn();
            }

            yield return null;
        }

    }

}
