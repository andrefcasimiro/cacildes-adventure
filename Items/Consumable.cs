using UnityEngine;

namespace AF
{
    public enum Stat
    {
        None,
        Health,
        Magic,
        Stamina,
        Reputation
    }

    public enum Attribute
    {
        None,
        Vitality,
        Intelligence,
        Endurance,
        Strength,
        Dexterity,
        Arcane,
        Charisma,
        Luck,
    }

    public enum Action
    {
        Restore,
        Regenerate,
        Increase,
        RemoveStatus
    }

    public enum ValueType
    {
        Point,
        Percentage
    }

    [CreateAssetMenu(menuName = "Item / New Consumable")]
    public class Consumable : Item
    {
        public Stat stat = Stat.None;

        public Attribute attribute = Attribute.None;

        public Action action;

        public float value;

        public ValueType valueType;

        public StatusEffect statusEffectToRemove;

        [Header("Regenerate Settings")]
        public float effectDuration = 0f;

        [Header("FX")]
        public GameObject particleOnConsume;
        public AudioClip sfxOnConsume;

        public void OnConsume()
        {
            PlayerInventoryManager.instance.RemoveItem(this, 1);

            if (sfxOnConsume != null)
            {
                BGMManager.instance.PlaySound(sfxOnConsume, null);
            }

            if (particleOnConsume != null)
            {
                Instantiate(particleOnConsume, FindObjectOfType<Player>(true).transform);
            }

            if (action == Action.RemoveStatus)
            {
                if (statusEffectToRemove != null)
                {
                    PlayerStatsManager.instance.RemoveStatusEffect(statusEffectToRemove);
                    return;
                }
            }

            if (effectDuration != 0f)
            {
                PlayerStatsManager.instance.AddConsumable(this);
                return;
            }

            if (stat == Stat.Health)
            {
                if (valueType == ValueType.Percentage)
                {
                    PlayerStatsManager.instance.RestoreHealthPercentage(value);
                    return;
                }

                if (valueType == ValueType.Point)
                {
                    PlayerStatsManager.instance.RestoreHealthPoints(value);
                    return;
                }
            }
        }
    }

}
