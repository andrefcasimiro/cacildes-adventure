using System.Collections;
using UnityEngine;

namespace AF
{
    public class Bonfire : MonoBehaviour
    {
        [Header("UI")]
        public UIDocumentBonfireMenu uiDocumentBonfireMenu;
        public LocalizedText bonfireName;

        public GenericTrigger bonfireTrigger;
        public Transform playerTransformRef;

        public GameObject fireFx;

        ThirdPersonController thirdPersonController;
        PlayerComponentManager playerComponentManager;
        PlayerInventory playerInventory;

        private Animator playerAnimator;

        public bool canBeTravelledTo = true;

        CursorManager cursorManager;

        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;


        private void Awake()
        {
            cursorManager = FindObjectOfType<CursorManager>(true);
        }

        private void Start()
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            playerComponentManager = playerInventory.GetComponent<PlayerComponentManager>();
            thirdPersonController = playerComponentManager.GetComponent<ThirdPersonController>();
            playerAnimator = playerComponentManager.GetComponent<Animator>();
        }

        public void UnlockBonfire(string bonfireName)
        {
            if (bonfiresDatabase.unlockedBonfires.Contains(bonfireName))
            {
                return;
            }

            bonfiresDatabase.unlockedBonfires.Add(bonfireName);
        }

        public void ActivateBonfire()
        {
            playerAnimator.Play("Idle Walk Run Blend");
            playerInventory.ReplenishItems();

            // Cure playerManager
            playerComponentManager.CurePlayer();

            if (canBeTravelledTo)
            {
                UnlockBonfire(bonfireName.GetEnglishText());
            }

            // Find all active enemies in scene
            var allEnemiesInScene = FindObjectsOfType<EnemyManager>();
            foreach (var enemy in allEnemiesInScene)
            {
                if (enemy.GetComponent<EnemyBossController>() != null)
                {
                    continue;
                }

                enemy.enabled = true;
                enemy.Revive();
            }

            fireFx.gameObject.SetActive(true);

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            playerComponentManager.transform.position = playerTransformRef.transform.position;
            var rot = fireFx.transform.position - playerComponentManager.transform.position;
            rot.y = 0;
            playerComponentManager.transform.rotation = Quaternion.LookRotation(rot);

            playerComponentManager.isInBonfire = true;

            playerComponentManager.GetComponent<Animator>().SetBool("IsSitting", true);

            thirdPersonController.LockCameraPosition = true;

            StartCoroutine(ShowBonfireUI());
        }

        public void ExitBonfire()
        {
            // Save Game
            // SaveSystem.instance.SaveGameData(); //bonfireName.GetText());

            uiDocumentBonfireMenu.gameObject.SetActive(false);

            playerComponentManager.GetComponent<Animator>().SetBool("IsSitting", false);

            playerComponentManager.isInBonfire = false;

            StartCoroutine(PutFireOut());

            playerComponentManager.CurePlayer();
        }

        IEnumerator ShowBonfireUI()
        {
            yield return new WaitForSeconds(0.5f);

            uiDocumentBonfireMenu.gameObject.SetActive(true);
        }

        IEnumerator PutFireOut()
        {
            yield return new WaitForSeconds(0.5f);
            fireFx.gameObject.SetActive(false);
            bonfireTrigger.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.25f);

            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();

            cursorManager.HideCursor();
        }
    }
}
