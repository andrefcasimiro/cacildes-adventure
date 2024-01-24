namespace AF
{
    public class CharacterPoise : CharacterAbstractPoise
    {
        public int maxPoiseHits = 3;

        public override int GetMaxPoiseHits()
        {
            return maxPoiseHits;
        }
    }
}
