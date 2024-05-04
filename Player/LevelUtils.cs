using UnityEngine;

namespace AF
{
    public static class LevelUtils
    {

        public static int GetRequiredExperienceForLevel(int level)
        {
            // The formula is common for both methods, so merge them into one
            float requiredExperience = (level + level - 1) * 1.25f * 100;
            // Round to the nearest integer
            return Mathf.RoundToInt(requiredExperience);
        }


    }
}
