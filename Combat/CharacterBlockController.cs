using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterBlockController : MonoBehaviour
    {
        public CharacterBaseManager characterManager;

        public bool isBlocking = false;

        [Header("Settings")]
        [Tooltip("The effectivness of the shield. If 1f, the shield will not give any bonus. If higher, the shield is less effective.")]
        public float blockMultiplier = 1.1f;

        [Header("Unity Events")]
        public UnityEvent onBlockDamageEvent;

        public void SetIsBlocking(bool value)
        {
            isBlocking = value;
        }

        public void BlockAttack(Damage damage)
        {
            characterManager.characterPosture.TakePostureDamage((int)(damage.postureDamage * blockMultiplier));

            onBlockDamageEvent?.Invoke();
        }

        public bool CanBlockDamage(Damage damage)
        {
            if (!isBlocking)
            {
                return false;
            }

            return (characterManager.characterPosture.currentPostureDamage + (int)(damage.postureDamage * blockMultiplier)) < characterManager.characterPosture.maxPostureDamage;
        }

    }

}
