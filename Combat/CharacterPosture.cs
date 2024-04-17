namespace AF
{
    public class CharacterPosture : CharacterAbstractPosture
    {
        public int maxPostureDamage = 100;

        public override int GetMaxPostureDamage()
        {
            return maxPostureDamage;
        }

        public override float GetPostureDecreateRate()
        {
            return 1f;
        }
    }

}
