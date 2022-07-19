using System;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EquipmentGraphicsHandler : MonoBehaviour, ISaveable
    {
        [HideInInspector] public Transform leftHand;
        [HideInInspector] public Transform rightHand;

        [HideInInspector] public WeaponInstance weaponInstance;
        [HideInInspector] public ShieldInstance shieldInstance;

        [HideInInspector] public GameObject weaponGraphic;
        [HideInInspector] public GameObject shieldGraphic;

        public List<string> helmetNakedParts = new List<string>
        {
            "Hair",
            "Head"
        };

        public List<string> armorNakedParts = new List<string>
        {
            "Torso",
        };

        public List<string> gauntletsNakedParts = new List<string>
        {
            "Left Arm",
            "Right Arm",
        };

        public List<string> legwearNakedParts = new List<string>
        {
            "Hip",
            "Left Leg",
            "Right Leg"
        };

        RuntimeAnimatorController playerDefaultAnimator;

        void Start()
        {
            HandRef[] handRefs = FindObjectsOfType<HandRef>(true);
            foreach (HandRef handRef in handRefs)
            {
                if (handRef.isLeft)
                {
                    leftHand = handRef.transform;
                }
                else
                {
                    rightHand = handRef.transform;
                }
            }

            playerDefaultAnimator = FindObjectOfType<Player>().animator.runtimeAnimatorController;

            InitializeEquipment();
        }

        public void InitializeEquipment()
        {

            if (PlayerInventoryManager.instance.currentWeapon != null)
            {
                Equip(PlayerInventoryManager.instance.currentWeapon);
            }

            if (PlayerInventoryManager.instance.currentShield != null)
            {
                Equip(PlayerInventoryManager.instance.currentShield);
            }

            if (PlayerInventoryManager.instance.currentHelmet != null)
            {
                Equip(PlayerInventoryManager.instance.currentHelmet);
            }

            if (PlayerInventoryManager.instance.currentChest != null)
            {
                Equip(PlayerInventoryManager.instance.currentChest);
            }

            if (PlayerInventoryManager.instance.currentLegwear != null)
            {
                Equip(PlayerInventoryManager.instance.currentLegwear);
            }

            if (PlayerInventoryManager.instance.currentGauntlets != null)
            {
                Equip(PlayerInventoryManager.instance.currentGauntlets);
            }

            if (PlayerInventoryManager.instance.currentAccessory1 != null)
            {
                EquipAccessory(PlayerInventoryManager.instance.currentAccessory1, 0);
            }

            if (PlayerInventoryManager.instance.currentAccessory2 != null)
            {
                EquipAccessory(PlayerInventoryManager.instance.currentAccessory2, 1);
            }
        }

        public void Equip(Weapon weaponToEquip)
        {
            UnequipWeapon();

            if (weaponToEquip == null)
            {
                return;
            }

            PlayerInventoryManager.instance.currentWeapon = weaponToEquip;

            if (weaponToEquip.graphic != null)
            {
                weaponGraphic = Instantiate(weaponToEquip.graphic, leftHand);
            }

            this.weaponInstance = weaponGraphic.GetComponentInChildren<WeaponInstance>(true);

            FindObjectOfType<Player>().animator.runtimeAnimatorController = weaponToEquip.animatorOverrideController;
        }

        public void Equip(Shield shieldToEquip)
        {
            UnequipShield();

            if (shieldToEquip == null)
            {
                return;
            }

            PlayerInventoryManager.instance.currentShield = shieldToEquip;

            shieldGraphic = Instantiate(shieldToEquip.graphic, rightHand);
            this.shieldInstance = shieldGraphic.GetComponentInChildren<ShieldInstance>(true);

            shieldGraphic.SetActive(false);
        }

        public void Equip(Armor armor)
        {
            if (armor == null)
            {
                return;
            }

            ArmorSlot armorType = armor.armorType;

            if (armorType == ArmorSlot.Head)
            {
                PlayerInventoryManager.instance.currentHelmet = armor;
            }
            else if (armorType == ArmorSlot.Chest)
            {
                PlayerInventoryManager.instance.currentChest = armor;
            }
            else if (armorType == ArmorSlot.Arms)
            {
                PlayerInventoryManager.instance.currentGauntlets = armor;
            }
            else if (armorType == ArmorSlot.Legs)
            {
                PlayerInventoryManager.instance.currentLegwear = armor;
            }

            ReloadEquipmentGraphics();
        }

        public void EquipAccessory(Accessory accessory, int index)
        {
            if (accessory == null) return;

            if (index == 0)
            {
                PlayerInventoryManager.instance.currentAccessory1 = accessory;
            }
            else
            {
                PlayerInventoryManager.instance.currentAccessory2 = accessory;
            }
        }

        public void UnequipWeapon()
        {
            Destroy(weaponGraphic);

            this.weaponInstance = null;

            PlayerInventoryManager.instance.currentWeapon = PlayerInventoryManager.instance.defaultUnarmedWeapon;
            FindObjectOfType<Player>().animator.runtimeAnimatorController = playerDefaultAnimator;
        }

        public void UnequipShield()
        {
            Destroy(shieldGraphic);

            this.shieldInstance = null;

            PlayerInventoryManager.instance.currentShield = null;
        }

        public void UnequipArmorSlot(ArmorSlot armorType)
        {
            Player player = FindObjectOfType<Player>(true);

            if (armorType == ArmorSlot.Head)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var helmet = PlayerInventoryManager.instance.currentHelmet;
                    if (helmet != null && helmet.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }
                }
                
                PlayerInventoryManager.instance.currentHelmet = null;
            }
            else if (armorType == ArmorSlot.Chest)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var chest = PlayerInventoryManager.instance.currentChest;

                    if (chest != null && chest.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }
                }

                PlayerInventoryManager.instance.currentChest = null;
            }
            else if (armorType == ArmorSlot.Arms)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var gauntlets = PlayerInventoryManager.instance.currentGauntlets;
                    if (gauntlets != null && gauntlets.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }
                }

                PlayerInventoryManager.instance.currentGauntlets = null;
            }
            else if (armorType == ArmorSlot.Legs)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var legwear = PlayerInventoryManager.instance.currentLegwear;

                    if (legwear != null && legwear.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }
                }

                PlayerInventoryManager.instance.currentLegwear = null;
            }

            ReloadEquipmentGraphics();
        }

        public void UnequipAccessory(int slotIndex)
        {
            if (slotIndex == 0)
            {
                PlayerInventoryManager.instance.currentAccessory1 = null;
            }
            else
            {
                PlayerInventoryManager.instance.currentAccessory2 = null;
            }
        }

        void ReloadEquipmentGraphics()
        {
            Player player = FindObjectOfType<Player>(true);

            foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
            {
                // HELMET
                var helmet = PlayerInventoryManager.instance.currentHelmet;
                if (helmet == null)
                {
                    if (helmetNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (helmet.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in helmet.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }

                // ARMOR
                var chest = PlayerInventoryManager.instance.currentChest;
                if (chest == null)
                {
                    if (armorNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (chest.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in chest.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }

                // GAUNTLETS
                var gauntlets = PlayerInventoryManager.instance.currentGauntlets;
                if (gauntlets == null)
                {
                    if (gauntletsNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (gauntlets.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in gauntlets.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }

                // LEGWEAR
                var legwear = PlayerInventoryManager.instance.currentLegwear;
                if (legwear == null)
                {
                    if (legwearNakedParts.IndexOf(t.gameObject.name) != -1)
                    {
                        t.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (legwear.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(true);
                    }

                    foreach (string graphicNameToHide in legwear.graphicNamesToHide)
                    {
                        if (t.gameObject.name == graphicNameToHide)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        #region Graphic Manipulation
        public void ToggleShield(bool value)
        {
            if (this.shieldInstance != null)
            {
                this.shieldInstance.gameObject.SetActive(value);
            }
        }
        public void ShowShield()
        {
            if (this.shieldInstance != null)
            {
                this.shieldInstance.gameObject.SetActive(true);
            }
        }
        public void HideShield()
        {
            if (this.shieldInstance != null)
            {
                this.shieldInstance.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Serialization
        public void OnGameLoaded(GameData gameData)
        {
            PlayerEquipmentData playerEquipmentData = gameData.playerEquipmentData;

            if (!String.IsNullOrEmpty(playerEquipmentData.weaponName))
            {
                Equip(PlayerInventoryManager.instance.GetItem(playerEquipmentData.weaponName) as Weapon);
            }
            else
            {
                UnequipWeapon();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.shieldName))
            {
                Equip(PlayerInventoryManager.instance.GetItem(playerEquipmentData.shieldName) as Shield);
            }
            else
            {
                UnequipShield();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.helmetName))
            {
                Equip(PlayerInventoryManager.instance.GetItem(playerEquipmentData.helmetName) as Armor);
            }
            else
            {
                UnequipArmorSlot(ArmorSlot.Head);
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.chestName))
            {
                Equip(PlayerInventoryManager.instance.GetItem(playerEquipmentData.chestName) as Armor);
            }
            else
            {
                UnequipArmorSlot(ArmorSlot.Chest);
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.legwearName))
            {
                Equip(PlayerInventoryManager.instance.GetItem(playerEquipmentData.legwearName) as Armor);
            }
            else
            {
                UnequipArmorSlot(ArmorSlot.Legs);
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.gauntletsName))
            {
                Equip(PlayerInventoryManager.instance.GetItem(playerEquipmentData.gauntletsName) as Armor);
            }
            else
            {
                UnequipArmorSlot(ArmorSlot.Arms);
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.accessory1Name))
            {
                EquipAccessory(PlayerInventoryManager.instance.GetItem(playerEquipmentData.accessory1Name) as Accessory, 0);
            }
            else
            {
                UnequipAccessory(0);
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.accessory2Name))
            {
                EquipAccessory(PlayerInventoryManager.instance.GetItem(playerEquipmentData.accessory2Name) as Accessory, 1);
            }
            else
            {
                UnequipAccessory(1);
            }
        }

        #endregion

    }
}
