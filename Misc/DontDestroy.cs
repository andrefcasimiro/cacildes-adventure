using UnityEngine;

namespace AF
{

    public class DontDestroy : MonoBehaviour
    {
        public static DontDestroy instance;

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

            DontDestroyOnLoad(this.gameObject);
        }

    }

}