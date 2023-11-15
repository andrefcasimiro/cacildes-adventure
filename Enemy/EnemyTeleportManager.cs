using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class EnemyTeleportManager : MonoBehaviour
    {

        [Header("Options")]
        [Range(0, 100)] public int chanceToTeleport = 50;
        [Range(0, 100)] public int chanceToUseBuff = 25;

        [Header("FX")]
        public GameObject teleportFX;
        public GameObject teleportFinishFX;

        CharacterManager characterManager => GetComponent<CharacterManager>();
        BossTeleportableTransform[] teleportableTransform;
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

        private void Awake()
        {
            teleportableTransform = FindObjectsOfType<BossTeleportableTransform>(true);
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            initialY = transform.position.y;
        }

        public void BeginTeleport()
        {
            characterManager.animator.Play(teleportAnimationName);

            StartCoroutine(BeginTeleportCoroutine());
        }

        IEnumerator BeginTeleportCoroutine()
        {
            onTeleport.Invoke();
            characterManager.characterController.enabled = false;

            Instantiate(teleportFX, transform.position, Quaternion.identity);

            /*characterManager.enemyNegativeStatusController.HideHUD();
            characterManager.enemyPostureController.HideHUD();
            characterManager.enemyHealthController.HideHUD();

            yield return new WaitForSeconds(delayBeforeDisablingRenderers);
            characterManager.enemyHealthController.DisableHealthHitboxes();
*/
            yield return null;

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
            /*
                        characterManager.enemyNegativeStatusController.ShowHUD();
                        characterManager.enemyPostureController.ShowHUD();
            */
            if (string.IsNullOrEmpty(reapperAnimationName))
            {
                //characterManager.animator.Play(characterManager.hashCombatting);
            }
            else
            {
                characterManager.animator.Play(reapperAnimationName);
            }

            characterManager.characterController.enabled = true;

            /*
                        characterManager.enemyHealthController.EnableHealthHitboxes();
                        characterManager.enemyHealthController.ShowHUD();

                        foreach (var skinnedMeshRenderer in enemyMeshRenderers)
                        {
                            skinnedMeshRenderer.enabled = true;
                        }

                        characterManager.FacePlayer();
            */
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

                characterManager.agent.SetDestination(transform.position);
                return;
            }

            Transform newTeleportPosition = teleportableTransform[Random.Range(0, teleportableTransform.Length)].transform;

            NavMesh.SamplePosition(newTeleportPosition.transform.position, out rightHit, 5f, NavMesh.AllAreas);
            transform.position = new Vector3(rightHit.position.x, initialY, rightHit.position.z);
            characterManager.agent.SetDestination(transform.position);
        }

    }
}
