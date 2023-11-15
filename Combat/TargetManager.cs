using UnityEngine;

namespace AF.Combat
{

    public class TargetManager : MonoBehaviour
    {
        Transform m_currentTarget;
        public Transform CurrentTarget
        {
            get
            {
                return m_currentTarget;
            }

            set
            {
                m_currentTarget = value;

                value.TryGetComponent(out targetCharacter);
            }
        }

        ICharacter targetCharacter;

        public void SetTarget(Transform target)
        {
            CurrentTarget = target;
        }

        public bool IsTargetBusy()
        {
            if (targetCharacter == null)
            {
                return false;
            }

            return targetCharacter.IsBusy();
        }
    }
}