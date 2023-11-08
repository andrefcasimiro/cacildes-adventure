using UnityEngine;

namespace AF
{
    public class HealthStatManager : MonoBehaviour
    {
        public int baseHealth = 100;
        public float levelMultiplier = 3.25f;

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        public int GetMaxHealth()
        {
            return baseHealth + (int)(Mathf.RoundToInt((Player.instance.vitality + equipmentGraphicsHandler.vitalityBonus) * levelMultiplier));
        }

        public void SubtractAmount(float amount)
        {
            Player.instance.currentHealth = Mathf.Clamp(Player.instance.currentHealth - amount, 0, GetMaxHealth());
        }

        public void RestoreHealthPercentage(float amount)
        {
            var percentage = (this.GetMaxHealth() * amount / 100);
            var nextValue = Mathf.Clamp(Player.instance.currentHealth + percentage, 0, this.GetMaxHealth());

            Player.instance.currentHealth = nextValue;
        }

        public void RestoreHealthPoints(float amount)
        {
            var nextValue = Mathf.Clamp(Player.instance.currentHealth + amount, 0, this.GetMaxHealth());

            Player.instance.currentHealth = nextValue;
        }

        public float GetHealthPointsForGivenVitality(int vitality)
        {
            return baseHealth + (int)(Mathf.Ceil(vitality * levelMultiplier));
        }

        public float GetExtraAttackBasedOnCurrentHealth()
        {
            var percentage = (Player.instance.currentHealth * 100 / GetMaxHealth()) * 0.01;

            if (percentage > 0.9)
            {
                return 0;
            }
            else if (percentage > 0.8)
            {
                return 0.05f;
            }
            else if (percentage > 0.7)
            {
                return 0.1f;
            }
            else if (percentage > 0.6)
            {
                return 0.2f;
            }
            else if (percentage > 0.5)
            {
                return 0.5f;
            }
            else if (percentage > 0.4)
            {
                return 0.6f;
            }
            else if (percentage > 0.3)
            {
                return 0.8f;
            }
            else if (percentage > 0.2)
            {
                return 1.2f;
            }
            else if (percentage > 0.1)
            {
                return 1.5f;
            }
            else if (percentage > 0)
            {
                return 2f;
            }

            return 0f;
        }


    }

}
