using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace AF
{

    public class EquipmentGraphicsHandler : MonoBehaviour, ISaveable
    {
        [HideInInspector] public Transform leftHand;
        [HideInInspector] public Transform rightHand;

        public PlayerWeaponHitbox leftUnarmedHitbox;
        public PlayerWeaponHitbox rightUnarmedHitbox;

        public PlayerWeaponHitbox leftWeaponHitbox;
        public PlayerWeaponHitbox rightWeaponHitbox;
        public PlayerWeaponHitbox leftFootHitbox;
        public PlayerWeaponHitbox rightFootHitbox;

        public GameObject leftWeaponGraphic;
        public GameObject rightWeaponGraphic;
        public GameObject shieldGraphic;
        
        public GameObject leftWeaponGraphicBack;
        public GameObject leftWeaponGraphicHolster;
        public GameObject rightWeaponGraphicHolster;

        public Transform backRef;
        public Transform leftHolsterRef;
        public Transform rightHolsterRef;

        StarterAssets.ThirdPersonController thirdPersonController => GetComponent<StarterAssets.ThirdPersonController>();

        LockOnManager lockOnManager;

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
            lockOnManager = FindObjectOfType<LockOnManager>(true);

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

            playerDefaultAnimator = GetComponent<Animator>().runtimeAnimatorController;

            InitializeEquipment();
        }

        public void InitializeEquipment()
        {
            ReloadEquipmentGraphics();

            if (Player.instance.equippedWeapon != null)
            {
                EquipWeapon(Player.instance.equippedWeapon);
            }

            if (Player.instance.equippedShield != null)
            {
                EquipShield(Player.instance.equippedShield);
            }

            if (Player.instance.equippedHelmet != null)
            {
                EquipHelmet(Player.instance.equippedHelmet);
            }

            if (Player.instance.equippedArmor != null)
            {
                EquipArmor(Player.instance.equippedArmor);
            }

            if (Player.instance.equippedLegwear != null)
            {
                EquipLegwear(Player.instance.equippedLegwear);
            }

            if (Player.instance.equippedGauntlets != null)
            {
                EquipGauntlet(Player.instance.equippedGauntlets);
            }

            if (Player.instance.equippedAccessory != null)
            {
                EquipAccessory(Player.instance.equippedAccessory);
            }
        }

        public void EquipWeapon(Weapon weaponToEquip)
        {
            UnequipWeapon();

            if (weaponToEquip == null)
            {
                return;
            }

            Player.instance.equippedWeapon = weaponToEquip;

            if (weaponToEquip.graphic != null)
            {
                if (weaponToEquip.isDualWielded)
                {
                    leftWeaponGraphic = Instantiate(weaponToEquip.graphic, leftHand);
                    rightWeaponGraphic = Instantiate(weaponToEquip.graphic, rightHand);
                    rightWeaponGraphic.GetComponentInChildren<LeftWeaponPivot>(true).gameObject.SetActive(false);
                    rightWeaponGraphic.GetComponentInChildren<RightWeaponPivot>(true).gameObject.SetActive(true);
                }
                else
                {
                    leftWeaponGraphic = Instantiate(weaponToEquip.graphic, leftHand);
                }

                if (weaponToEquip.useBackRef)
                {
                    leftWeaponGraphicBack = Instantiate(weaponToEquip.graphic, backRef);
                    leftWeaponGraphicBack.SetActive(false);
                }
                else if (weaponToEquip.useHolsterRef)
                {
                    leftWeaponGraphicHolster = Instantiate(weaponToEquip.graphic, leftHolsterRef);
                    leftWeaponGraphicHolster.SetActive(false);

                    if (weaponToEquip.isDualWielded)
                    {
                        rightWeaponGraphicHolster = Instantiate(weaponToEquip.graphic, rightHolsterRef);
                        rightWeaponGraphicHolster.SetActive(false);
                    }
                }
            }

            if (leftWeaponGraphic != null)
            {
                this.leftWeaponHitbox = leftWeaponGraphic.GetComponentInChildren<PlayerWeaponHitbox>(true);

                if (weaponToEquip.isDualWielded)
                {
                    var rightWeapon = rightWeaponGraphic.GetComponentInChildren<RightWeaponPivot>(true);
                    this.rightWeaponHitbox = rightWeapon.transform.GetComponentInChildren<PlayerWeaponHitbox>(true);
                }

            }
            if (rightWeaponGraphic != null && weaponToEquip.isDualWielded == false)
            {
                this.rightWeaponHitbox = rightWeaponGraphic.GetComponentInChildren<PlayerWeaponHitbox>(true);
            }

            if (weaponToEquip.animatorOverrideController != null)
            {
                GetComponent<Animator>().runtimeAnimatorController = weaponToEquip.animatorOverrideController;
            }
            else
            {
                GetComponent<Animator>().runtimeAnimatorController = playerDefaultAnimator;
            }

            if (lockOnManager.isLockedOn)
            {
                GetComponent<Animator>().SetBool(lockOnManager.hashIsLockedOn, true);
            }

            DeactivateAllHitboxes();
        }

        public void EquipShield(Shield shieldToEquip)
        {
            UnequipShield();

            if (shieldToEquip == null)
            {
                return;
            }

            Player.instance.equippedShield = shieldToEquip;

            shieldGraphic = Instantiate(shieldToEquip.graphic, rightHand);
            shieldGraphic.SetActive(false);
        }

        #region Helmet
        public void EquipHelmet(Helmet helmetToEquip)
        {
            if (helmetToEquip == null)
            {
                return;
            }

            UnequipHelmet();

            if (helmetToEquip != Player.instance.equippedHelmet)
            {
                Player.instance.equippedHelmet = helmetToEquip;
            }

            ReloadEquipmentGraphics();
        }

        public void UnequipHelmet()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                var helmetToUnequip = Player.instance.equippedHelmet;

                if (helmetToUnequip != null)
                {
                    if (helmetToUnequip.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (helmetToUnequip.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            Player.instance.equippedHelmet = null;
            ReloadEquipmentGraphics();
        }
        #endregion

        #region Armor
        public void EquipArmor(ArmorBase armorToEquip)
        {
            if (armorToEquip == null)
            {
                return;
            }

            UnequipArmor();

            if (armorToEquip != Player.instance.equippedArmor)
            {
                Player.instance.equippedArmor = armorToEquip;
            }

            ReloadEquipmentGraphics();
        }

        public void UnequipArmor()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                var armorToUnequip = Player.instance.equippedArmor;

                if (armorToUnequip != null)
                {
                    if (armorToUnequip.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (armorToUnequip.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            Player.instance.equippedArmor = null;
            ReloadEquipmentGraphics();
        }
        #endregion

        #region Gauntlets
        public void EquipGauntlet(Gauntlet gauntletToEquip)
        {
            if (gauntletToEquip == null)
            {
                return;
            }

            UnequipGauntlet();

            if (gauntletToEquip != Player.instance.equippedGauntlets)
            {
                Player.instance.equippedGauntlets = gauntletToEquip;
            }

            ReloadEquipmentGraphics();
        }

        public void UnequipGauntlet()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                var gauntletToUnequip = Player.instance.equippedGauntlets;

                if (gauntletToUnequip != null)
                {
                    if (gauntletToUnequip.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (gauntletToUnequip.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            Player.instance.equippedGauntlets = null;
            ReloadEquipmentGraphics();
        }
        #endregion

        #region Legwear
        public void EquipLegwear(Legwear legwearToEquip)
        {
            if (legwearToEquip == null)
            {
                return;
            }

            UnequipLegwear();

            if (legwearToEquip != Player.instance.equippedLegwear)
            {
                Player.instance.equippedLegwear = legwearToEquip;
            }

            ReloadEquipmentGraphics();
        }

        public void UnequipLegwear()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                var legwearToUnequip = Player.instance.equippedLegwear;

                if (legwearToUnequip != null)
                {
                    if (legwearToUnequip.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (legwearToUnequip.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            Player.instance.equippedLegwear = null;
            ReloadEquipmentGraphics();
        }
        #endregion

        public void EquipAccessory(Accessory accessoryToEquip)
        {
            if (accessoryToEquip == null)
            {
                return;
            }

            if (accessoryToEquip != Player.instance.equippedAccessory)
            {
                UnequipAccessory();

                Player.instance.equippedAccessory = accessoryToEquip;
            }
        }

        public void UnequipWeapon()
        {
            if (leftWeaponGraphic != null)
            {
                Destroy(leftWeaponGraphic);
                this.leftWeaponHitbox = leftUnarmedHitbox;
            }

            if (rightWeaponGraphic != null)
            {
                Destroy(rightWeaponGraphic);
                this.rightWeaponHitbox = rightUnarmedHitbox;
            }

            if (leftWeaponGraphicBack != null)
            {
                Destroy(leftWeaponGraphicBack);
            }

            if (leftWeaponGraphicHolster != null)
            {
                Destroy(leftWeaponGraphicHolster);
            }

            if (rightWeaponGraphicHolster != null)
            {
                Destroy(rightWeaponGraphicHolster);
            }

            Player.instance.equippedWeapon = null;
            GetComponent<Animator>().runtimeAnimatorController = playerDefaultAnimator;
            if (lockOnManager.isLockedOn)
            {
                GetComponent<Animator>().SetBool(lockOnManager.hashIsLockedOn, true);
            }
        }

        public void UnequipShield()
        {
            Destroy(shieldGraphic);

            Player.instance.equippedShield = null;
        }

        public void UnequipAccessory()
        {
            Player.instance.equippedAccessory = null;
        }

        void ReloadEquipmentGraphics()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                // HELMET
                var helmet = Player.instance.equippedHelmet;
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
                var chest = Player.instance.equippedArmor;
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
                var gauntlets = Player.instance.equippedGauntlets;
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
                var legwear = Player.instance.equippedLegwear;
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

        #region Hitboxes

        /// <summary>
        /// Animation Event
        /// </summary>
        public void HitStart()
        {
            if (this.leftWeaponHitbox != null)
            {
                ActivateLeftWeaponHitbox();
            }
            else if (this.rightWeaponHitbox != null)
            {
                ActivateRightWeaponHitbox();
            }
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void HitEnd()
        {
            DeactivateAllHitboxes();
        }

        public void ActivateLeftWeaponHitbox()
        {
            thirdPersonController.DisableCharacterRotation();

            if (this.leftWeaponHitbox == null)
            {
                leftUnarmedHitbox.EnableHitbox();

                return;
            }

            leftWeaponHitbox.EnableHitbox();
        }

        public void ActivateRightWeaponHitbox()
        {
            thirdPersonController.DisableCharacterRotation();

            if (this.rightWeaponHitbox == null)
            {
                rightUnarmedHitbox.EnableHitbox();

                return;
            }

            rightWeaponHitbox.EnableHitbox();
        }

        public void ActivateLeftFootHitbox()
        {
            thirdPersonController.DisableCharacterRotation();

            if (this.leftFootHitbox == null)
            {
                return;
            }

            leftFootHitbox.EnableHitbox();
        }

        public void ActivateRightFootHitbox()
        {
            thirdPersonController.DisableCharacterRotation();

            if (this.rightFootHitbox == null)
            {
                return;
            }

            rightFootHitbox.EnableHitbox();
        }

        public void DeactivateLeftWeaponHitbox()
        {
            if (this.leftWeaponHitbox == null)
            {
                this.leftUnarmedHitbox.DisableHitbox();
                return;
            }

            leftWeaponHitbox.DisableHitbox();
        }

        public void DeactivateRightWeaponHitbox()
        {
            if (this.rightWeaponHitbox == null)
            {
                this.rightUnarmedHitbox.DisableHitbox();
                return;
            }

            rightWeaponHitbox.DisableHitbox();
        }

        public void DeactivateLeftFootHitbox()
        {
            if (this.leftFootHitbox == null)
            {
                return;
            }

            leftFootHitbox.DisableHitbox();
        }

        public void DeactivateRightFootHitbox()
        {
            if (this.rightFootHitbox == null)
            {
                return;
            }

            rightFootHitbox.DisableHitbox();
        }

        public void DeactivateAllHitboxes()
        {
            DeactivateLeftWeaponHitbox();
            DeactivateRightWeaponHitbox();
            DeactivateLeftFootHitbox();
            DeactivateRightFootHitbox();
        }

        public void HideWeapons()
        {
            if (this.leftWeaponGraphic != null)
            {
                this.leftWeaponGraphic.SetActive(false);


                if (this.leftWeaponGraphicBack)
                {
                    this.leftWeaponGraphicBack.SetActive(true);
                }

                if (leftWeaponGraphicHolster != null)
                {
                    this.leftWeaponGraphicHolster.SetActive(true);
                }

                if (rightWeaponGraphicHolster != null)
                {
                    this.rightWeaponGraphicHolster.SetActive(true);
                }
            }

            if (this.rightWeaponGraphic != null)
            {
                this.rightWeaponGraphic.SetActive(false);
            }
        }

        public void ShowWeapons()
        {
            if (this.leftWeaponGraphic != null)
            {
                this.leftWeaponGraphic.SetActive(true);

                if (this.leftWeaponGraphicBack)
                {
                    this.leftWeaponGraphicBack.SetActive(false);
                }

                if (leftWeaponGraphicHolster != null)
                {
                    this.leftWeaponGraphicHolster.SetActive(false);
                }

                if (rightWeaponGraphicHolster != null)
                {
                    this.rightWeaponGraphicHolster.SetActive(false);
                }
            }

            if (this.rightWeaponGraphic != null)
            {
                this.rightWeaponGraphic.SetActive(true);
            }

        }

        #endregion

        #region Serialization
        public void OnGameLoaded(GameData gameData)
        {
            PlayerEquipmentData playerEquipmentData = gameData.playerEquipmentData;

            if (!String.IsNullOrEmpty(playerEquipmentData.weaponName))
            {
                var weapon = Resources.Load<Weapon>("Items/Weapons/" + playerEquipmentData.weaponName);
                EquipWeapon(weapon);
            }
            else
            {
                UnequipWeapon();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.shieldName))
            {
                var shield = Resources.Load<Shield>("Items/Shields/" + playerEquipmentData.shieldName);
                EquipShield(shield);
            }
            else
            {
                UnequipShield();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.helmetName))
            {
                var helmet = Resources.Load<Helmet>("Items/Helmets/" + playerEquipmentData.helmetName);
                EquipHelmet(helmet);
            }
            else
            {
                UnequipHelmet();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.chestName))
            {
                var armor = Resources.Load<ArmorBase>("Items/Armors/" + playerEquipmentData.chestName);
                EquipArmor(armor);
            }
            else
            {
                UnequipArmor();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.legwearName))
            {
                var legwear = Resources.Load<Legwear>("Items/Legwears/" + playerEquipmentData.legwearName);
                EquipLegwear(legwear);
            }
            else
            {
                UnequipLegwear();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.gauntletsName))
            {
                var gauntlets = Resources.Load<Gauntlet>("Items/Gauntlets/" + playerEquipmentData.gauntletsName);
                EquipGauntlet(gauntlets);
            }
            else
            {
                UnequipGauntlet();
            }

            if (!String.IsNullOrEmpty(playerEquipmentData.accessory1Name))
            {
                var accessory = Resources.Load<Accessory>("Items/Accessories/" + playerEquipmentData.accessory1Name);
                EquipAccessory(accessory);
            }
            else
            {
                UnequipAccessory();
            }

        }

        #endregion

    }
}
