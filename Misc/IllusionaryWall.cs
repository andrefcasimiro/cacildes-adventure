
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class IllusionaryWall : MonoBehaviour
    {
        public bool hasBeenHit;
        Material material;
        public Material fadeMaterial;
        public float alpha;
        public float fadeTimer = 2.5f;
        bool hasDeactivatedColliders = false;

        public UnityEvent onHit;

        // Scene Refs
        Soundbank soundbank;

        [Header("Options")]
        public bool ignorePlayerDodging = false;

        private void Start()
        {
            this.material = Instantiate(fadeMaterial);
        }

        public void SetHasBeenHit()
        {
            hasBeenHit = true;
        }

        private void Update()
        {
            if (hasBeenHit)
            {
                FadeIllusionaryWall();
            }
        }

        public void FadeIllusionaryWall()
        {
            GetComponent<MeshRenderer>().material = this.material;

            alpha = material.color.a;
            alpha = alpha - Time.deltaTime / fadeTimer;
            Color c = new Color(1, 1, 1, alpha);
            material.color = c;

            if (onHit != null)
            {
                onHit.Invoke();
            }

            if (hasDeactivatedColliders == false)
            {
                foreach (var col in GetComponents<Collider>())
                {
                    col.enabled = false;
                }

                hasDeactivatedColliders = true;

                GetSoundBank().PlaySound(GetSoundBank().illusionaryWallSound);
            }

            if (alpha <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (hasBeenHit || ignorePlayerDodging)
            {
                return;
            }

            if (other.CompareTag("Player") && other.TryGetComponent<PlayerManager>(out var playerManager))
            {
                if (playerManager.dodgeController.isDodging)
                {
                    hasBeenHit = true;
                }
            }
        }

        Soundbank GetSoundBank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank;
        }

    }

}
