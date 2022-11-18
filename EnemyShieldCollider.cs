using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyShieldCollider : MonoBehaviour
    {
        EnemyBlockController enemyBlockController => GetComponentInParent<EnemyBlockController>();
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

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player" && playerCombatController.isCombatting)
            {
                if (cooldown < maxCooldown) { return; }
                cooldown = 0f;
                Instantiate(enemyBlockController.blockParticleEffect, this.transform.position, Quaternion.identity);
            }
        }
    }

}