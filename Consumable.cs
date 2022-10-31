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

        public bool canFavorite = true;

        public void OnConsume()
        {
            PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>(true);
            playerInventory.RemoveItem(this, 1);

            if (sfxOnConsume != null)
            {
                BGMManager.instance.PlaySound(sfxOnConsume, null);
            }

            if (particleOnConsume != null)
            {
                Instantiate(particleOnConsume, GameObject.FindWithTag("Player").transform);
            }

            if (action == Action.RemoveStatus)
            {
                if (statusEffectToRemove != null)
                {
                    var targetAppliedStatus = Player.instance.appliedStatus.Find(x => x.statusEffect == statusEffectToRemove);
                    if (targetAppliedStatus == null)
                    {
                        return;
                    }

                    playerInventory.GetComponent<PlayerStatusManager>().RemoveAppliedStatus(targetAppliedStatus);
                    return;
                }
            }

            if (effectDuration != 0f)
            {
                // PlayerStatsManager.instance.AddConsumable(this);
                return;
            }

            if (stat == Stat.Health)
            {
                if (valueType == ValueType.Percentage)
                {
                    HealthStatManager healthStatManager = FindObjectOfType<HealthStatManager>(true);
                    healthStatManager.RestoreHealthPercentage((int)Mathf.RoundToInt(value));
                    return;
                }

                if (valueType == ValueType.Point)
                {
                    HealthStatManager healthStatManager = FindObjectOfType<HealthStatManager>(true);
                    healthStatManager.RestoreHealthPoints((int)Mathf.RoundToInt(value));
                    return;
                }
            }
        }
    }

}
