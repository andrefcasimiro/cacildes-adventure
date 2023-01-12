using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyShieldCollider : MonoBehaviour
    {
        EnemyManager enemyManager => GetComponentInParent<EnemyManager>();
        PlayerCombatController playerCombatController;

        float maxCooldown = 1f;
        float cooldown = Mathf.Infinity;

        private void Start()
        {
            playerCombatController = FindObjectOfType<PlayerCombatController>(true);    
        }

        private void Update()
        {
            if (cooldown < maxCooldown) { cooldown += Time.deltaTime;  }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "PlayerWeapon")
            {
                if (cooldown < maxCooldown) { return; }
                cooldown = 0f;
                Instantiate(enemyManager.blockParticleEffect, this.transform.position, Quaternion.identity);
            }
        }
    }

}