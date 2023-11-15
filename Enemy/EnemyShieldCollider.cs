using UnityEngine;

namespace AF
{

    public class EnemyShieldCollider : MonoBehaviour
    {
        CharacterManager characterManager => GetComponentInParent<CharacterManager>();

        float maxCooldown = 1f;
        float cooldown = Mathf.Infinity;

        private void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            /*
            if (other.gameObject.tag == "PlayerWeapon" && characterManager.enemyBlockController != null && characterManager.enemyBlockController.IsBlocking())
            {
                if (cooldown < maxCooldown)
                {
                    return;
                }

                cooldown = 0f;
                Instantiate(characterManager.enemyBlockController.blockParticleEffect, this.transform.position, Quaternion.identity);
            }*/
        }
    }
}
