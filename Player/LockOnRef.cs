using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class LockOnRef : MonoBehaviour
    {
        [HideInInspector] public CharacterManager characterManager;

        private void Start()
        {
            characterManager = GetComponentInParent<CharacterManager>(true);
        }

        public bool CanLockOn()
        {
            if (characterManager != null)
            {
                return false;// characterManager.enemyHealthController.currentHealth > 0;
            }

            // Must be dummy target then
            return true;
        }
    }

}
