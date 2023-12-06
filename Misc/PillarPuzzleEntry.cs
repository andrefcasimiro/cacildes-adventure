using System.Collections;
using System.Collections.Generic;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class PillarPuzzleEntry : SwitchListener
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public int pillarOrder = 1;

        PillarPuzzleManager pillarPuzzleManager;

        public bool isActivated = false;

        public Material activatedMaterial;
        public Material deactivatedMaterial;

        MeshRenderer meshRenderer => GetComponent<MeshRenderer>();

        public AudioClip activateSfx;

        GenericTrigger genericTrigger;

        public UnityEvent onFailEvent;

        private void Awake()
        {
            genericTrigger = GetComponentInChildren<GenericTrigger>(true);

            deactivatedMaterial = Instantiate(meshRenderer.material);
            activatedMaterial = Instantiate(activatedMaterial);

            pillarPuzzleManager = FindObjectOfType<PillarPuzzleManager>(true);
        }

        private void Start()
        {
            Refresh();
        }

        public void OnActivatePillar()
        {
            if (isActivated)
            {
                return;
            }

            if (pillarPuzzleManager.currentScore + 1 == pillarOrder)
            {
                // Correct order
                bgmManager.PlaySound(activateSfx, null);
                pillarPuzzleManager.IncreaseScore();

                MarkAsActive();
            }
            else
            {
                // Incorrect order
                pillarPuzzleManager.ResetPuzzle();

                onFailEvent?.Invoke();

                StartCoroutine(ResetStateAfterFailure());
            }
        }

        IEnumerator ResetStateAfterFailure()
        {
            yield return new WaitForSeconds(.5f);


            genericTrigger.gameObject.SetActive(true);
        }

        public void MarkAsActive()
        {
            isActivated = true;

            meshRenderer.material = activatedMaterial;

            genericTrigger.gameObject.SetActive(false);
        }

        public void ResetState()
        {
            isActivated = false;
            genericTrigger.gameObject.SetActive(false);
            genericTrigger.gameObject.SetActive(true);
            meshRenderer.material = deactivatedMaterial;
        }


        // If puzzle is solved, always show the activated state
        public override void Refresh()
        {
            if (SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == true)
            {
                MarkAsActive();
            }
        }

    }

}
