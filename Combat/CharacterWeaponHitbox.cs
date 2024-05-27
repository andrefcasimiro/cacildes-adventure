using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public UnityEvent onWeaponSpecial;

        [Header("Character Weapon Addons")]
        public CharacterTwoHandRef characterTwoHandRef;
        public CharacterWeaponBuffs characterWeaponBuffs;

        // Scene References
        Soundbank soundbank;

        // Internal flags
        bool canPlayHitSfx = true;

        List<BoxCollider> ownColliders = new();

        private void Awake()
        {
            ownColliders = GetComponents<BoxCollider>()?.ToList();
        }

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

            if (ownColliders?.Count > 1)
            {
                foreach (var collider in ownColliders)
                {
                    collider.enabled = false;
                }
            }

            damageReceiversHit.Clear();
            onCloseHitbox?.Invoke();
        }

        bool IsCharacterConfused()
        {
            return character != null && character.isConfused;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (tagsToIgnore.Contains(other.tag) && !IsCharacterConfused())
            {
                return;
            }

            if (character.GetAttackDamage() == null && !IsCharacterConfused())
            {
                return;
            }

            other.TryGetComponent(out DamageReceiver damageReceiver);

            if (IsCharacterConfused() ||
                damageReceiver != null
                && damageReceiver?.character != character
                && damageReceiversHit.Contains(damageReceiver) == false)
            {

                if (IsCharacterConfused() && character.damageReceiver != null)
                {
                    damageReceiver = character.damageReceiver;
                }

                damageReceiver.HandleIncomingDamage(character, (incomingDamage) =>
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
                }, IsCharacterConfused());

                damageReceiversHit.Add(damageReceiver);

                if (character is PlayerManager playerManager)
                {
                    if (playerManager.playerBlockController.isCounterAttacking)
                    {
                        playerManager.playerBlockController.onCounterAttack?.Invoke();
                    }

                    damageReceiver?.health?.onDamageFromPlayer?.Invoke();

                    if (this.weapon != null && damageReceiver?.health?.weaponRequiredToKill != null && damageReceiver.health.weaponRequiredToKill == this.weapon)
                    {
                        damageReceiver.health.hasBeenHitWithRequiredWeapon = true;
                    }
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
