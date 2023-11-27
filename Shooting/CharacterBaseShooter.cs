using UnityEngine;

namespace AF.Shooting
{
    public abstract class CharacterBaseShooter : MonoBehaviour
    {
        public readonly int hashFireBow = Animator.StringToHash("Shoot");
        public readonly int hashCast = Animator.StringToHash("Cast");
        public readonly int hashIsAiming = Animator.StringToHash("IsAiming");

        [Header("Components")]
        public CharacterBaseManager characterBaseManager;

        public abstract bool CanShoot();

        public abstract void CastSpell();

        public abstract void FireArrow();
    }
}
