using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ShowLevelUpScreenUI : EventBase
    {
        public UIDocumentLevelUpScreenUI uIDocumentLevelUpScreenUi;

        public override IEnumerator Dispatch()
        {
            yield return null;

            yield return uIDocumentLevelUpScreenUi.OpenLevelUpScreen();
        }
    }

}
