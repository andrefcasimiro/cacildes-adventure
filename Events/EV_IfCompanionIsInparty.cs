using System.Collections;
using System.Linq;

namespace AF
{
    public class EV_IfCompanionIsInparty : EV_Condition
    {
        public Companion companion;

        public override IEnumerator Dispatch()
        {

            object companionInstance = null; //Player.instance.companions.FirstOrDefault(c => c.companionId == companion.companionId);
            bool companionIsInParty = companionInstance != null;
            yield return DispatchConditionResults(companionIsInParty);
        }
    }
}
