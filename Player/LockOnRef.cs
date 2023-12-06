using UnityEngine;

namespace AF
{

    public class LockOnRef : MonoBehaviour
    {
        [Header("Components")]
        public CharacterManager characterManager;

        public bool CanLockOn()
        {
            if (characterManager != null)
            {
                return characterManager.health.GetCurrentHealth() > 0;
            }

            // Must be dummy target then
            return true;
        }
    }

}
