using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class PlayerBackstabController : MonoBehaviour
    {
        public readonly int hashBackstabExecution = Animator.StringToHash("Backstab Execution");

        public LayerMask characterLayer;

        [Header("Options")]
        public float backStabAngle = 90f;

        [Header("FX")]
        public UnityEvent onBackstab;

        [Header("Components")]
        public PlayerManager playerManager;
        public Transform playerEyesRef;

        public bool PerformBackstab()
        {
            CharacterManager enemy = GetPossibleTarget();

            if (enemy != null && CanBackstab(enemy))
            {
                enemy.characterPosture.isStunned = true;
                enemy.damageReceiver.waitingForBackstab = true;

                // If backstab sucess
                enemy.damageReceiver.HandleIncomingDamage(playerManager, (incomeDamage) =>
                {
                    playerManager.transform.rotation = enemy.transform.rotation;
                    playerManager.playerComponentManager.DisablePlayerControlAndRegainControlAfterResetStates();
                    enemy.targetManager.SetTarget(playerManager);

                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashBackstabExecution);
                    Invoke(nameof(PlayDelayedBackstab), 0.8f);
                });

                return true;
            }

            return false;
        }

        void PlayDelayedBackstab()
        {
            onBackstab?.Invoke();
        }

        CharacterManager GetPossibleTarget()
        {
            // Get the forward direction of the player
            Vector3 playerForward = playerEyesRef.transform.forward;

            // Cast a ray from the player's chest forward
            if (Physics.Raycast(playerEyesRef.transform.position, playerForward, out RaycastHit hit, 1f, characterLayer))
            {
                float angle = Vector3.Angle(playerEyesRef.transform.forward, hit.transform.forward);
                if (hit.transform != null && angle < backStabAngle)
                {
                    hit.transform.TryGetComponent<CharacterManager>(out var character);

                    return character;
                }
            }

            return null;
        }

        public bool CanBackstab(CharacterManager target)
        {
            if (!target.damageReceiver.canBeBackstabbed)
            {
                return false;
            }

            if (target.characterPosture.isStunned)
            {
                return false;
            }

            return true;
        }
    }
}
