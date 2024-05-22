namespace AF
{
    public class CharacterPosture : CharacterAbstractPosture
    {
        public int maxPostureDamage = 100;
        public GameSession gameSession;

        public override int GetMaxPostureDamage()
        {
            return Utils.ScaleWithCurrentNewGameIteration(maxPostureDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
        }

        public override float GetPostureDecreateRate()
        {
            return 1f;
        }
    }

}
