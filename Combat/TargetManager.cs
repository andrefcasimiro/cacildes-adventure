using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class TargetManager : MonoBehaviour
    {
        public UnityEvent onTargetSet_Event;

        public CharacterBaseManager currentTarget;


        public void SetTarget(CharacterBaseManager target)
        {
            if (currentTarget == target)
            {
                return;
            }

            currentTarget = target;
            onTargetSet_Event?.Invoke();
        }

        public bool IsTargetBusy()
        {
            if (currentTarget == null)
            {
                return false;
            }

            return currentTarget.IsBusy();
        }
    }
}
