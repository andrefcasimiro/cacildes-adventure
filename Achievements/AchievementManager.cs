using System.Collections;
using UnityEngine;

namespace AF
{
    public class AchievementManager : MonoBehaviour
    {

        public static AchievementManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }


    }
}