using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace AF.Arena
{
    public class ArenaPowerup : MonoBehaviour
    {
        public StatusEffect[] possibleStatusEffects;
        public float statusEffectDuration = 15f;
        public float timeBeforeDestroying = 15f;

        [Header("Localization")]
        //has picked up powerup:
        public LocalizedString hasPickedPowereUp_LocalizedString;

        private void OnEnable()
        {
            IEnumerator DestroySelf()
            {
                yield return new WaitForSeconds(timeBeforeDestroying);

                if (this.gameObject.activeSelf)
                {
                    Destroy(this.gameObject);
                }
            }

            StartCoroutine(DestroySelf());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (possibleStatusEffects == null || possibleStatusEffects.Length == 0)
            {
                return;
            }

            if (other.TryGetComponent<CharacterBaseManager>(out var character) && character.statusController != null)
            {
                StatusEffect chosenStatusEffect = possibleStatusEffects[Random.Range(0, possibleStatusEffects.Length)];
                if (chosenStatusEffect != null)
                {
                    if (character.statusController.statusEffectResistances.ContainsKey(chosenStatusEffect))
                    {
                        character.statusController.statusEffectResistances[chosenStatusEffect] = statusEffectDuration;
                    }
                    else
                    {
                        character.statusController.statusEffectResistances.Add(chosenStatusEffect, statusEffectDuration);
                    }

                    FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include).ShowNotification(
                        character.name + " " + hasPickedPowereUp_LocalizedString.GetLocalizedString() + " " + chosenStatusEffect.appliedName, null);

                    character.statusController.InflictStatusEffect(
                        chosenStatusEffect, statusEffectDuration, true);
                }
            }

            Destroy(this.gameObject);
        }
    }
}
