namespace AF
{
    public class CharacterPoise : CharacterAbstractPoise
    {
        public int maxPoiseHits = 3;

        public bool hasHyperArmor = false;

        public override void ResetStates()
        {
            hasHyperArmor = false;
        }

        public override bool CanCallPoiseDamagedEvent()
        {
            if (hasHyperArmor)
            {
                return false;
            }

            return true;
        }

        public override int GetMaxPoiseHits()
        {
            return maxPoiseHits;
        }
    }
}
