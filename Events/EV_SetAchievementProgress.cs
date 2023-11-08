using System.Collections;

namespace AF
{
    public class EV_SetAchievementProgress : EventBase
    {
        public Achievement achievementToAward;

        public override IEnumerator Dispatch()
        {
            achievementToAward.AwardAchievement();

            yield return null;
        }
    }
}
