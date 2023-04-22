using UnityEngine;

namespace AF
{

    public class EnemyShieldCollider : MonoBehaviour
    {
        EnemyManager enemyManager => GetComponentInParent<EnemyManager>();

        float maxCooldown = 1f;
        float cooldown = Mathf.Infinity;

        private void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            /*
            if (other.gameObject.tag == "PlayerWeapon" && enemyManager.enemyBlockController != null && enemyManager.enemyBlockController.IsBlocking())
            {
                if (cooldown < maxCooldown)
                {
                    return;
                }

                cooldown = 0f;
                Instantiate(enemyManager.enemyBlockController.blockParticleEffect, this.transform.position, Quaternion.identity);
            }*/
        }
    }
}
