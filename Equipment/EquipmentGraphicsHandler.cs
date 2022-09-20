using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AF
{

    public class EquipmentGraphicsHandler : MonoBehaviour, ISaveable
    {
        [HideInInspector] public Transform leftHand;
        [HideInInspector] public Transform rightHand;

        [HideInInspector] public WeaponInstance leftHandWeaponInstance;
        [HideInInspector] public WeaponInstance rightHandWeaponInstance;


        [HideInInspector] public ShieldInstance shieldInstance;

        [HideInInspector] public GameObject leftWeaponGraphic;
        [HideInInspector] public GameObject rightWeaponGraphic;

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
            ReloadEquipmentGraphics();

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
                if (weaponToEquip.isDualWielded)
                {
                    leftWeaponGraphic = Instantiate(weaponToEquip.graphic, leftHand);
                    rightWeaponGraphic = Instantiate(weaponToEquip.graphic, rightHand);
                }
                else
                {
                    leftWeaponGraphic = Instantiate(weaponToEquip.graphic, leftHand);
                }
            }

            if (leftWeaponGraphic != null)
            {
                this.leftHandWeaponInstance = leftWeaponGraphic.GetComponentInChildren<WeaponInstance>(true);
            }
            if (rightWeaponGraphic != null)
            {
                this.rightHandWeaponInstance = rightWeaponGraphic.GetComponentInChildren<WeaponInstance>(true);
            }

            if (weaponToEquip.animatorOverrideController != null)
            {
                FindObjectOfType<Player>(true).animator.runtimeAnimatorController = weaponToEquip.animatorOverrideController;
            }
            else
            {
                FindObjectOfType<Player>(true).animator.runtimeAnimatorController = playerDefaultAnimator;
            }
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

            UnequipArmorSlot(armorType);


            if (armorType == ArmorSlot.Head && armor != PlayerInventoryManager.instance.currentHelmet)
            {
                PlayerInventoryManager.instance.currentHelmet = armor;

                armor.OnEquip();
            }
            else if (armorType == ArmorSlot.Chest && armor != PlayerInventoryManager.instance.currentChest)
            {
                PlayerInventoryManager.instance.currentChest = armor;

                armor.OnEquip();
            }
            else if (armorType == ArmorSlot.Arms && armor != PlayerInventoryManager.instance.currentGauntlets)
            {
                PlayerInventoryManager.instance.currentGauntlets = armor;

                armor.OnEquip();
            }
            else if (armorType == ArmorSlot.Legs && armor != PlayerInventoryManager.instance.currentLegwear)
            {
                PlayerInventoryManager.instance.currentLegwear = armor;

                armor.OnEquip();
            }

            PlayerStatsManager.instance.HandleEquipmentChanges();

            ReloadEquipmentGraphics();
        }

        public void EquipAccessory(Accessory accessory, int index)
        {
            if (accessory == null) return;

            if (index == 0 && accessory != PlayerInventoryManager.instance.currentAccessory1)
            {
                UnequipAccessory(0);

                PlayerInventoryManager.instance.currentAccessory1 = accessory;
                accessory.OnEquip();
            }
            else if (index == 1 && accessory != PlayerInventoryManager.instance.currentAccessory2)
            {
                UnequipAccessory(1);

                PlayerInventoryManager.instance.currentAccessory2 = accessory;
                accessory.OnEquip();
            }

            PlayerStatsManager.instance.HandleEquipmentChanges();
        }

        public void UnequipWeapon()
        {
            if (leftWeaponGraphic != null)
            {
                Destroy(leftWeaponGraphic);
                this.leftHandWeaponInstance = null;
            }

            if (rightWeaponGraphic != null)
            {
                Destroy(rightWeaponGraphic);
                this.rightHandWeaponInstance = null;
            }

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
            GameObject player = GameObject.FindWithTag("Player");

            if (armorType == ArmorSlot.Head)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var helmet = PlayerInventoryManager.instance.currentHelmet;
                    if (helmet != null)
                    {
                        if (helmet.graphicNameToShow == t.gameObject.name)
                        {
                            helmet.OnUnequip();
                            t.gameObject.SetActive(false);
                        }

                        if (helmet.graphicNamesToHide.Contains(t.gameObject.name)) {
                            t.gameObject.SetActive(true);
                        }
                    }
                }
                
                PlayerInventoryManager.instance.currentHelmet = null;
            }
            else if (armorType == ArmorSlot.Chest)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var chest = PlayerInventoryManager.instance.currentChest;

                    if (chest != null) {
                        if (chest.graphicNameToShow == t.gameObject.name) {
                            chest.OnUnequip();
                            t.gameObject.SetActive(false);
                        }

                        if (chest.graphicNamesToHide.Contains(t.gameObject.name)) {
                            t.gameObject.SetActive(true);
                        }
                    }
                }

                PlayerInventoryManager.instance.currentChest = null;
            }
            else if (armorType == ArmorSlot.Arms)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var gauntlets = PlayerInventoryManager.instance.currentGauntlets;
                    if (gauntlets != null)
                    {
                        if (gauntlets.graphicNameToShow == t.gameObject.name)
                        {
                            gauntlets.OnUnequip();

                            t.gameObject.SetActive(false);
                        }

                        if (gauntlets.graphicNamesToHide.Contains(t.gameObject.name))
                        {
                            t.gameObject.SetActive(true);
                        }
                    }
                }

                PlayerInventoryManager.instance.currentGauntlets = null;
            }
            else if (armorType == ArmorSlot.Legs)
            {
                foreach (Transform t in player.GetComponentsInChildren<Transform>(true))
                {
                    var legwear = PlayerInventoryManager.instance.currentLegwear;

                    if (legwear != null)
                    {
                        if (legwear.graphicNameToShow == t.gameObject.name)
                        {
                            legwear.OnUnequip();
                            t.gameObject.SetActive(false);
                        }

                        if (legwear.graphicNamesToHide.Contains(t.gameObject.name))
                        {
                            t.gameObject.SetActive(true);
                        }
                    }
                }

                PlayerInventoryManager.instance.currentLegwear = null;
            }

            PlayerStatsManager.instance.HandleEquipmentChanges();

            ReloadEquipmentGraphics();
        }

        public void UnequipAccessory(int slotIndex)
        {

            if (slotIndex == 0)
            {
                if (PlayerInventoryManager.instance.currentAccessory1 != null)
                {
                    PlayerInventoryManager.instance.currentAccessory1.OnUnequip();
                }

                PlayerInventoryManager.instance.currentAccessory1 = null;
            }
            else
            {
                if (PlayerInventoryManager.instance.currentAccessory2 != null)
                {
                    PlayerInventoryManager.instance.currentAccessory2.OnUnequip();
                }

                PlayerInventoryManager.instance.currentAccessory2 = null;
            }

            PlayerStatsManager.instance.HandleEquipmentChanges();
        }

        void ReloadEquipmentGraphics()
        {
            GameObject player = GameObject.FindWithTag("Player");

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

            PlayerStatsManager.instance.HandleEquipmentChanges();
        }

        #endregion

    }
}
