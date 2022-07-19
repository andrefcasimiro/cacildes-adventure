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
    }

    public enum Action
    {
        Restore,
        Regenerate,
        Increase
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

        [Header("Regenerate Settings")]
        public float effectDuration = 0f;
        public float ratePerSecond;

        [Header("FX")]
        public GameObject particleOnConsume;
        public AudioClip sfxOnConsume;

        public void OnConsume()
        {
            if (sfxOnConsume != null)
            {
                SFXManager sfxManager = FindObjectOfType<SFXManager>(true);

                if (sfxManager != null)
                {
                    sfxManager.PlaySound(sfxOnConsume, null);
                }
            }

            if (particleOnConsume != null)
            {
                Instantiate(particleOnConsume, FindObjectOfType<Player>(true).transform);
            }

            if (effectDuration != 0f)
            {
                PlayerStatsManager.instance.appliedConsumables.Add(this);
            }
            else if (stat != Stat.None)
            {
               switch (stat)
                {
                    case Stat.Health:
                        if (valueType == ValueType.Percentage)
                        {
                            PlayerStatsManager.instance.RestoreHealthPercentage(value);
                        }
                        else
                        {
                            PlayerStatsManager.instance.RestoreHealthPoints(value);
                        }
                        break;
                    default:
                        return;
                }
            }

            PlayerInventoryManager.instance.currentItems.Remove(this);
        }
    }

}
