using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ManagePlayerHUD : EventBase
    {
        public bool show;

        public override IEnumerator Dispatch()
        {
            FindObjectOfType<UIDocumentPlayerHUDV2>(true).gameObject.SetActive(show);

            yield return null;
        }
    }

}
