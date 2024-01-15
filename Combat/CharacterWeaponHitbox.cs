using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterWeaponHitbox : MonoBehaviour
    {
        [Header("Weapon")]
        public Weapon weapon;

        [Header("Trails")]
        public TrailRenderer trailRenderer;
        public BoxCollider hitCollider => GetComponent<BoxCollider>();

        [Header("Components")]
        public CharacterBaseManager character;
        public Soundbank soundbank;

        [Header("Tags To Ignore")]
        public List<string> tagsToIgnore = new();

        [Header("SFX")]
        public AudioClip swingSfx;
        public AudioClip hitSfx;
        public AudioSource combatAudioSource;

        readonly List<DamageReceiver> damageReceiversHit = new();

        [Header("Events")]
        public UnityEvent onOpenHitbox;
        public UnityEvent onCloseHitbox;

        // Internal flags
        bool canPlayHitSfx = true;

        void Start()
        {
            DisableHitbox();
        }

        public void ShowWeapon()
        {
            gameObject.SetActive(true);
        }

        public void HideWeapon()
        {
            gameObject.SetActive(false);
        }

        public void EnableHitbox()
        {
            canPlayHitSfx = true;

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            if (swingSfx != null && soundbank != null)
            {
                soundbank.PlaySound(swingSfx, combatAudioSource);
            }

            onOpenHitbox?.Invoke();
        }

        public void DisableHitbox()
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }

            damageReceiversHit.Clear();
            onCloseHitbox?.Invoke();
        }

        public void OnTriggerEnter(Collider other)
        {

            if (tagsToIgnore.Contains(other.tag))
            {
                return;
            }

            if (character.GetAttackDamage() == null)
            {
                return;
            }

            if (
                other.TryGetComponent(out DamageReceiver damageReceiver)
                && damageReceiver?.character != character
                && damageReceiversHit.Contains(damageReceiver) == false)
            {

                damageReceiver.HandleIncomingDamage(character, () =>
                {
                    if (hitSfx != null && canPlayHitSfx && character != null)
                    {
                        canPlayHitSfx = false;

                        if (soundbank != null)
                        {
                            soundbank.PlaySound(hitSfx, combatAudioSource);
                        }
                    }
                });

                damageReceiversHit.Add(damageReceiver);
            }
        }
    }
}
