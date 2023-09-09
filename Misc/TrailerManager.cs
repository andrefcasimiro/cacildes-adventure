using Cinemachine;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class TrailerManager : MonoBehaviour
    {
        public static TrailerManager instance;

        public Player.ItemEntry[] ownedItems;

        public bool alternateBetweenEquipments = false;
        public bool randomizeEquipment = false;

        [System.Serializable]
        public class Equipment
        {
            public Helmet helmet;

            public Armor armor;

            public Legwear legwear;

            public Gauntlet gauntlet;
        }

        public Equipment[] equipment;
        int loopIndex = 0;
        
        public float maxTimeBetweenEquipment = 2f;
        public float currentTimeBetweenEquipment = Mathf.Infinity;

        EquipmentGraphicsHandler equipmentGraphicsHandler;

        int sceneIndex = 0;

        int maxScenes = 20;

        GameObject dollyCamera;
        public bool enableDolly = false;
        public bool showHud = true;

        public Companion hugo;
        public Companion alcino;
        public Companion balbino;
        
        public void LoopEquipment()
        {
            if (currentTimeBetweenEquipment <= maxTimeBetweenEquipment)
            {
                currentTimeBetweenEquipment += Time.deltaTime;
                return;
            }

            if (loopIndex >= equipment.Length)
            {
                loopIndex = 0;
            }


            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = FindAnyObjectByType<EquipmentGraphicsHandler>(FindObjectsInactive.Include);
            }

            if (randomizeEquipment)
            {
                var helmet = equipment[Random.Range(0, equipment.Length)].helmet;
                var armor = equipment[Random.Range(0, equipment.Length)].armor;
                var gauntlet = equipment[Random.Range(0, equipment.Length)].gauntlet;
                var legwear = equipment[Random.Range(0, equipment.Length)].legwear;

                if (helmet != null)
                {
                    equipmentGraphicsHandler.EquipHelmet(helmet);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipHelmet();
                }

                if (armor != null)
                {
                    equipmentGraphicsHandler.EquipArmor(armor);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipArmor();
                }

                if (gauntlet != null)
                {
                    equipmentGraphicsHandler.EquipGauntlet(gauntlet);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipGauntlet();
                }

                if (legwear != null)
                {
                    equipmentGraphicsHandler.EquipLegwear(legwear);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipLegwear();
                }
            }
            else
            {
                var helmet = equipment[loopIndex].helmet;
                var armor = equipment[loopIndex].armor;
                var gauntlet = equipment[loopIndex].gauntlet;
                var legwear = equipment[loopIndex].legwear;

                if (helmet != null)
                {
                    equipmentGraphicsHandler.EquipHelmet(helmet);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipHelmet();
                }

                if (armor != null)
                {
                    equipmentGraphicsHandler.EquipArmor(armor);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipArmor();
                }

                if (gauntlet != null)
                {
                    equipmentGraphicsHandler.EquipGauntlet(gauntlet);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipGauntlet();
                }

                if (legwear != null)
                {
                    equipmentGraphicsHandler.EquipLegwear(legwear);
                }
                else
                {
                    equipmentGraphicsHandler.UnequipLegwear();
                }

                loopIndex++;
            }

            currentTimeBetweenEquipment = 0;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }
        /*
        private void Start()
        {
            var copiedOwnedItems = ownedItems.ToArray();
            foreach (var ownedItem in copiedOwnedItems)
            {
                Player.instance.ownedItems.Add(new Player.ItemEntry()
                {
                    item = Instantiate(ownedItem.item),
                    amount = ownedItem.amount,
                    usages = ownedItem.usages,
                });
            }
        }

        float memoizedFogDensity = 0;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                alternateBetweenEquipments = !alternateBetweenEquipments;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                randomizeEquipment = !randomizeEquipment;
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                sceneIndex++;
                if (sceneIndex >= maxScenes)
                {
                    sceneIndex = 0;
                }

                SceneManager.LoadScene(sceneIndex);
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                enableDolly = !enableDolly;

                if (enableDolly)
                {
                    if (dollyCamera == null)
                    {
                        dollyCamera = FindAnyObjectByType<CinemachineDollyCart>(FindObjectsInactive.Include).gameObject;
                    }

                    if (dollyCamera != null)
                    {
                        dollyCamera.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (dollyCamera != null)
                    {
                        dollyCamera.gameObject.SetActive(false);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                showHud = !showHud;

                if (showHud)
                {
                    FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include).transform.parent.gameObject.SetActive(true);

                }
                else
                {
                    FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include).transform.parent.gameObject.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                var t = FindAnyObjectByType<CompanionsSceneManager>(FindObjectsInactive.Include);
                t.AddCompanionToParty(hugo);
                t.AddCompanionToParty(alcino);
                t.AddCompanionToParty(balbino);
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                var t = FindAnyObjectByType<CinematicCamera>(FindObjectsInactive.Include);
                if (t.gameObject.activeSelf)
                {
                    RenderSettings.fogDensity = 0.02f;
                    t.gameObject.SetActive(false);
                    FindAnyObjectByType<PlayerComponentManager>(FindObjectsInactive.Include).gameObject.SetActive(true);
                }
                else
                {
                    RenderSettings.fogDensity = 0.02f;
                    t.gameObject.SetActive(true);
                    FindAnyObjectByType<PlayerComponentManager>(FindObjectsInactive.Include).gameObject.SetActive(false);
                }
            }


            if (alternateBetweenEquipments)
            {
                LoopEquipment();
            }
        }*/
    }
}
