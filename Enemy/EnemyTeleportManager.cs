using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class EnemyTeleportManager : MonoBehaviour
    {
        [Header("FX")]
        public GameObject teleportFX;
        public GameObject teleportFinishFX;

        EnemyManager enemyManager => GetComponent<EnemyManager>();
        BossTeleportableTransform[] teleportableTransform => FindObjectsOfType<BossTeleportableTransform>(true);
        SkinnedMeshRenderer[] enemyMeshRenderers => GetComponentsInChildren<SkinnedMeshRenderer>(true);

        public bool teleportNearPlayer = false;

        public string teleportAnimationName = "Teleport";
        public string reapperAnimationName = "";
        public float delayBeforeDisablingRenderers = 0.4f;
        public float intervalBetweenTeleports = 1f;

        public float initialY;

        Transform player;

        public UnityEvent onTeleport;
        public UnityEvent onReappear;
        public UnityEvent onReappearAfterHalfSecond;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            initialY = transform.position.y;
        }

        public void BeginTeleport()
        {
            enemyManager.animator.Play(teleportAnimationName);

            StartCoroutine(BeginTeleportCoroutine());
        }

        IEnumerator BeginTeleportCoroutine()
        {
            onTeleport.Invoke();

            Instantiate(teleportFX, transform.position, Quaternion.identity);

            enemyManager.enemyNegativeStatusController.HideHUD();
            enemyManager.enemyPostureController.HideHUD();

            yield return new WaitForSeconds(delayBeforeDisablingRenderers);
            enemyManager.enemyHealthController.DisableHealthHitboxes();


            if (teleportFinishFX != null)
            {
                Instantiate(teleportFinishFX, transform.position, Quaternion.identity);
            }
            foreach (var skinnedMeshRenderer in enemyMeshRenderers)
            {
                skinnedMeshRenderer.enabled = false;
            }

            ChoosePlatform();

            StartCoroutine(Reappear());
        }

        IEnumerator Reappear()
        {
            yield return new WaitForSeconds(intervalBetweenTeleports);

            Instantiate(teleportFX, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(.5f);

            enemyManager.enemyNegativeStatusController.ShowHUD();
            enemyManager.enemyPostureController.ShowHUD();

            if (string.IsNullOrEmpty(reapperAnimationName))
            {
                enemyManager.animator.Play(enemyManager.hashCombatting);
            }
            else
            {
                enemyManager.animator.Play(reapperAnimationName);
            }

            enemyManager.enemyHealthController.EnableHealthHitboxes();

            foreach (var skinnedMeshRenderer in enemyMeshRenderers)
            {
                skinnedMeshRenderer.enabled = true;
            }

            enemyManager.FacePlayer();

            onReappear.Invoke();

            yield return new WaitForSeconds(0.5f);

            onReappearAfterHalfSecond.Invoke();
        }

        public void ChoosePlatform()
        {
            NavMeshHit rightHit;

            if (teleportNearPlayer)
            {

                NavMesh.SamplePosition(player.transform.position, out rightHit, 5f, NavMesh.AllAreas);
                transform.position = new Vector3(rightHit.position.x, rightHit.position.y, rightHit.position.z);
                return;
            }

            Transform newTeleportPosition = teleportableTransform[Random.Range(0, teleportableTransform.Length)].transform;

            NavMesh.SamplePosition(newTeleportPosition.transform.position, out rightHit, 5f, NavMesh.AllAreas);
            transform.position = new Vector3(rightHit.position.x, initialY, rightHit.position.z);
        }

    }
}
