using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class Bonfire : MonoBehaviour
    {
        public GameObject fireFx;

        PlayerComponentManager playerComponentManager;
        
        public Transform playerTransformRef;

        public UIDocumentBonfireMenu uiDocumentBonfireMenu;

        public string bonfireName = "";

        public GenericTrigger bonfireTrigger;

        private void Start()
        {
            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
        }

        public void ActivateBonfire()
        {
            // Cure player
            playerComponentManager.CurePlayer();

            // Find all active enemies in scene
            var allEnemiesInScene = FindObjectsOfType<EnemyManager>();
            foreach (var enemy in allEnemiesInScene)
            {
                if (enemy.isBoss)
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

            StartCoroutine(ShowBonfireUI());
        }

        public void ExitBonfire()
        {

            // Save Game
            SaveSystem.instance.currentScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
            SaveSystem.instance.SaveGameData(bonfireName);

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

            Utils.HideCursor();
        }


    }

}
