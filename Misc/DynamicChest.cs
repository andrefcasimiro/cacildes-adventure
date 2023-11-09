using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DynamicChest : MonoBehaviour, ISaveable
    {

        public GameObject chestLid;

        public GenericTrigger openChestTrigger;
        public ItemPickup itemPickupTrigger;

        public AudioSource chestOpenAudioSource;

        bool isOpened = false;
        bool isOpeningLid = false;

        [Header("Lid Rotation")]
        public float rotationSpeed = 20;
        public float maxRotation = -105f;

        private void Start()
        {
            EvaluateIfShouldOpenLid();
            itemPickupTrigger.gameObject.SetActive(false);
        }

        private void _OpenChest()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
            itemPickupTrigger.gameObject.SetActive(true);
            openChestTrigger.enabled = false;
            isOpeningLid = true;
            StartCoroutine(RotateLid());
        }

        public void OpenChest()
        {
            chestOpenAudioSource.Play();
            _OpenChest();
        }


        private IEnumerator RotateLid()
        {
            while (isOpeningLid)
            {
                // Rotate the lid smoothly
                float currentRotation = chestLid.transform.localRotation.eulerAngles.x;
                float targetRotation = currentRotation + rotationSpeed * Time.deltaTime;

                if (targetRotation >= maxRotation)
                {
                    targetRotation = maxRotation;
                    isOpeningLid = false;
                }

                chestLid.transform.localRotation = Quaternion.Euler(targetRotation, 0, 0);

                yield return null;
            }
        }

        void EvaluateIfShouldOpenLid()
        {
            isOpened = SwitchManager.instance.GetSwitchCurrentValue(itemPickupTrigger.switchEntry);

            if (isOpened)
            {
                _OpenChest();

                itemPickupTrigger.gameObject.SetActive(false);
            }
        }

        public void OnGameLoaded(object gameData)
        {
            EvaluateIfShouldOpenLid();
        }

    }

}
