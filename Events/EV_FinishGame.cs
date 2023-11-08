using System.Collections;

namespace AF
{

    public class EV_FinishGame : EventBase
    {
        public Achievement achievementForFinishingGameWithCompanions;

        public override IEnumerator Dispatch()
        {
            if (Player.instance.companions.Count > 0)
            {
                achievementForFinishingGameWithCompanions.AwardAchievement();
            }

            yield return null;
        }
    }
}
