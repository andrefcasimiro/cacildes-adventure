using UnityEngine;

namespace AF
{
    public class WeaponCollisionFXManager : MonoBehaviour
    {
        public ParticleHelper woodFx;
        public AudioSourceHelper woodSfx;
        public ParticleHelper metalFx;
        public AudioSourceHelper metalSfx;
        public ParticleHelper grassFx;
        public AudioSourceHelper grassSfx;
        public ParticleHelper waterFx;
        public AudioSourceHelper waterSfx;

        public void EvaluateCollision(Collider collider, GameObject colliderGameObject)
        {

            if (collider.CompareTag("Water"))
            {
                waterFx.transform.position = collider.ClosestPoint(colliderGameObject.transform.position);
                waterFx.SafePlay();
                waterSfx.SafePlay();
            }
            if (collider.CompareTag("Wood"))
            {
                woodFx.transform.position = collider.ClosestPoint(colliderGameObject.transform.position);
                woodFx.SafePlay();
                woodSfx.SafePlay();
            }
            if (collider.CompareTag("Grass"))
            {
                grassFx.transform.position = collider.ClosestPoint(colliderGameObject.transform.position);
                grassFx.SafePlay();
                grassSfx.SafePlay();
            }
            if (collider.CompareTag("Stone"))
            {
                metalFx.transform.position = collider.ClosestPoint(colliderGameObject.transform.position);
                metalFx.SafePlay();
                metalSfx.SafePlay();
            }
        }
    }
}

