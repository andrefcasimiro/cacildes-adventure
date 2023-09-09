using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerLevelManager : MonoBehaviour
    {

        public int GetCurrentLevel()
        {
            return Player.instance.GetCurrentLevel();
        }

        public int GetRequiredExperienceForGivenLevel(int level)
        {
            return Mathf.RoundToInt(Mathf.Ceil((level + level - 1) * 1.25f * 100));
        }

        public float GetRequiredExperienceForNextLevel()
        {
            return Mathf.RoundToInt(Mathf.Ceil((this.GetCurrentLevel() + this.GetCurrentLevel() - 1) * 1.25f * 100));
        }

    }

}
