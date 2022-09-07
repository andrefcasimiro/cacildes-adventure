using UnityEngine;

namespace AF
{

    public class Character : MonoBehaviour
    {
        public readonly int hashMovementSpeed = Animator.StringToHash("movementSpeed");
        public readonly int hashDodging = Animator.StringToHash("Dodging");

        public Animator animator;
        [HideInInspector] public Healthbox healthbox => GetComponent<Healthbox>();

        public bool IsDodging()
        {
            return animator.GetBool(hashDodging);
        }
    }
}
