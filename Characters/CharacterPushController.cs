using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterPushController : MonoBehaviour
    {
        public CharacterManager characterManager;

        bool isPushed = false;

        public UnityEvent onPush_Begin;
        public UnityEvent onPush_End;

        // Call this method when the character gets hit by the force
        public void ApplyForceSmoothly(Vector3 forceDirection, float pushForce, float duration)
        {
            if (!isPushed)
            {
                onPush_Begin?.Invoke();
                StartCoroutine(ApplyForceCoroutine(forceDirection, pushForce, duration));
            }
        }

        private IEnumerator ApplyForceCoroutine(Vector3 forceDirection, float pushForce, float duration)
        {
            float elapsed = 0f;
            isPushed = true;
            while (elapsed < duration)
            {
                float forceMagnitude = Mathf.Lerp(pushForce, 0f, elapsed / duration);
                characterManager.characterController.Move(forceDirection * forceMagnitude * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            onPush_End?.Invoke();
            isPushed = false;
        }
    }
}
