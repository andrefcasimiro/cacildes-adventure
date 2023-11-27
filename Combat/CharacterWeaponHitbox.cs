using System.Collections.Generic;
using UnityEngine;

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

        [Header("Tags To Detect")]
        public List<string> tagsToDetect = new();

        readonly List<DamageReceiver> damageReceiversHit = new();

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
            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }
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
        }

        public void OnTriggerEnter(Collider other)
        {
            if (
                !tagsToDetect.Contains(other.gameObject.tag)
                || character.GetAttackDamage() == null
            )
            {
                return;
            }

            if (
                other.TryGetComponent(out DamageReceiver damageReceiver)
                && damageReceiversHit.Contains(damageReceiver) == false)
            {
                damageReceiver.HandleIncomingDamage(character);
                damageReceiversHit.Add(damageReceiver);
            }
        }
    }
}
