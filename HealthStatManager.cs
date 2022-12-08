using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class HealthStatManager : MonoBehaviour
    {
        public int baseHealth = 100;
        public float levelMultiplier = 3.25f;

        public int GetMaxHealth()
        {
            return baseHealth + (int)(Mathf.Ceil(Player.instance.vitality * levelMultiplier));
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
    }

}
