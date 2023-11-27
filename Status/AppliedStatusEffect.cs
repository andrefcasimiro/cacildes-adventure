namespace AF.StatusEffects
{
    [System.Serializable]
    public class AppliedStatusEffect
    {
        public StatusEffect statusEffect;

        public bool hasReachedTotalAmount;

        public float currentAmount;
    }
}
