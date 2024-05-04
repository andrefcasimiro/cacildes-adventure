namespace AF
{
    public static class HealthUtils
    {
        public static int GetExtraHealthBasedOnCompanionsInParty(int numberOfCompanionsInParty)
        {
            int scaleFactor = (int)(1 + (numberOfCompanionsInParty - 1) * 0.5f);
            return 1000 * numberOfCompanionsInParty * scaleFactor;
        }

    }
}
