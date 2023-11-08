using System.Collections;

namespace AF
{

    public class EV_JoinParty : EventBase
    {
        public Companion companionToJoin;

        public override IEnumerator Dispatch()
        {
            yield return null;

            FindObjectOfType<CompanionsSceneManager>(true).AddCompanionToParty(companionToJoin);
        }

    }

}
