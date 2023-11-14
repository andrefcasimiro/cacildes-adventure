using System.Collections;
using AF.Companions;

namespace AF
{

    public class EV_FinishGame : EventBase
    {
        public Achievement achievementForFinishingGameWithCompanions;
        public CompanionsDatabase companionsDatabase;

        public override IEnumerator Dispatch()
        {
            if (companionsDatabase.companions.Count > 0)
            {
                achievementForFinishingGameWithCompanions.AwardAchievement();
            }

            yield return null;
        }
    }
}
