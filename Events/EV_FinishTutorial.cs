using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_FinishTutorial : EventBase
    {
        public override IEnumerator Dispatch()
        {
    
            yield return null;

            Player.instance.ResetPlayerData();

            Player.instance.LoadScene(0, true);
        }
    }

}
