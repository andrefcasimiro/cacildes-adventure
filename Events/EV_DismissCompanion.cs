﻿using System.Collections;

namespace AF
{

    public class EV_DismissCompanion : EventBase
    {
        public Companion companionToDismiss;

        public override IEnumerator Dispatch()
        {
            yield return null;

            // FindObjectOfType<CompanionsSceneManager>(true).DismissCompanion(companionToDismiss);
        }

    }

}
