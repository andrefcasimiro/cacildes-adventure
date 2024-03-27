using System.Collections;
using System.Collections.Generic;
using AF.Health;
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
        public UnityEvent onDamageInflicted;

        [Header("Character Weapon Addons")]
        public CharacterTwoHandRef characterTwoHandRef;
        public CharacterWeaponBuffs characterWeaponBuffs;

        // Scene References
        Soundbank soundbank;

        // Internal flags
        bool canPlayHitSfx = true;

        void Start()
        {
            DisableHitbox();
        }

        public void ShowWeapon()
        {
            gameObject.SetActive(true);

            if (characterTwoHandRef != null)
            {
                characterTwoHandRef.EvaluateTwoHandingUpdate();
            }
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

            if (swingSfx != null && HasSoundbank())
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
                    onDamageInflicted?.Invoke();

                    if (hitSfx != null && canPlayHitSfx && character != null)
                    {
                        canPlayHitSfx = false;

                        if (HasSoundbank())
                        {
                            soundbank.PlaySound(hitSfx, combatAudioSource);
                        }
                    }
                });

                damageReceiversHit.Add(damageReceiver);

                if (character is PlayerManager)
                {
                    damageReceiver.health.onDamageFromPlayer?.Invoke();
                }
            }
        }

        bool HasSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);

                return soundbank != null;
            }

            return true;
        }

        public bool UseCustomTwoHandTransform()
        {
            return characterTwoHandRef != null;
        }
    }
}
