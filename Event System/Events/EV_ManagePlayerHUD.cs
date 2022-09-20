using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ManagePlayerHUD : EventBase
    {
        public bool show;

        public override IEnumerator Dispatch()
        {
            if (show)
            {
                FindObjectOfType<UIDocumentPlayerHUD>(true).Enable();
            }
            else
            {
                FindObjectOfType<UIDocumentPlayerHUD>(true).Disable();
            }

            yield return null;
        }
    }

}
