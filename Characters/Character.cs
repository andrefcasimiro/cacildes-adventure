using UnityEngine;

namespace AF
{

    [RequireComponent(typeof(Animator))]
    public class Character : MonoBehaviour
    {
        public readonly int hashMovementSpeed = Animator.StringToHash("movementSpeed");
        public readonly int hashDodging = Animator.StringToHash("Dodging");

        [HideInInspector] public Animator animator => GetComponent<Animator>();
        [HideInInspector] public Healthbox healthbox => GetComponent<Healthbox>();

        public bool IsDodging()
        {
            return animator.GetBool(hashDodging);
        }
    }
}
