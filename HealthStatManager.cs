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

        public void RestoreHealthPercentage(int currentHealthPercentage)
        {
            var percentage = (this.GetMaxHealth() * currentHealthPercentage / 100);
            var nextCurrentHealthValue = Mathf.Clamp(Player.instance.currentHealth + percentage, 0, this.GetMaxHealth());

            Player.instance.currentHealth = nextCurrentHealthValue;
        }

        public void RestoreHealthPoints(int healthPoints)
        {
            var nextCurrentHealthValue = Mathf.Clamp(Player.instance.currentHealth + healthPoints, 0, this.GetMaxHealth());

            Player.instance.currentHealth = nextCurrentHealthValue;
        }
    }

}
