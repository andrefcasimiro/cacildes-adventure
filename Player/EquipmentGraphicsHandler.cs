using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using AF.Stats;

namespace AF
{
    public class EquipmentGraphicsHandler : MonoBehaviour
    {
        public int BASE_NUMBER_OF_ACCESSORIES_THAT_CAN_EQUIP = 2;

        [System.Serializable]
        public class ExtraAccessorySlot
        {
            public SwitchEntry[] requiredSwitches;
        }

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

        public GameObject bow;


        [Header("Components")]
        public Animator playerAnimator;
        public StatsBonusController statsBonusController;

        public ThirdPersonController thirdPersonController;
        public PlayerCombatController playerCombatController;
        public LockOnManager lockOnManager;


        CinemachineImpulseSource impulseSource => GetComponent<CinemachineImpulseSource>();


        public List<ExtraAccessorySlot> extraAccessorySlots = new();

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

        [Header("UI Systems")]
        public NotificationManager notificationManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Transform References")]
        public Transform playerEquipmentRoot;

        void Start()
        {
            bow.gameObject.SetActive(false);

            InitializeEquipment();
        }


        public void InitializeEquipment()
        {
            ReloadEquipmentGraphics();

            if (equipmentDatabase.helmet != null)
            {
                EquipHelmet(equipmentDatabase.helmet);
            }

            if (equipmentDatabase.armor != null)
            {
                EquipArmor(equipmentDatabase.armor);
            }

            if (equipmentDatabase.legwear != null)
            {
                EquipLegwear(equipmentDatabase.legwear);
            }

            if (equipmentDatabase.gauntlet != null)
            {
                EquipGauntlet(equipmentDatabase.gauntlet);
            }

            for (int i = 0; i < equipmentDatabase.accessories.Length; i++)
            {
                EquipAccessory(equipmentDatabase.accessories[i], i);
            }
        }

        #region Helmet
        public void EquipHelmet(Helmet helmetToEquip)
        {
            if (helmetToEquip == null)
            {
                return;
            }

            UnequipHelmet();

            if (helmetToEquip != equipmentDatabase.helmet)
            {
                equipmentDatabase.EquipHelmet(helmetToEquip);
            }

            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipHelmet()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.helmet != null)
                {
                    if (equipmentDatabase.helmet.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.helmet.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipHelmet();

            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
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

            if (armorToEquip != equipmentDatabase.armor)
            {
                equipmentDatabase.EquipArmor(armorToEquip as Armor);
            }

            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipArmor()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.armor != null)
                {
                    if (equipmentDatabase.armor.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.armor.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipArmor();

            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
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

            if (gauntletToEquip != equipmentDatabase.gauntlet)
            {
                equipmentDatabase.EquipGauntlet(gauntletToEquip);
            }

            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipGauntlet()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.gauntlet != null)
                {
                    if (equipmentDatabase.gauntlet.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.gauntlet.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipGauntlet();
            ReloadEquipmentGraphics();
            statsBonusController.RecalculateEquipmentBonus();
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

            if (legwearToEquip != equipmentDatabase.legwear)
            {
                equipmentDatabase.EquipLegwear(legwearToEquip);
            }

            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipLegwear()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                if (equipmentDatabase.legwear != null)
                {
                    if (equipmentDatabase.legwear.graphicNameToShow == t.gameObject.name)
                    {
                        t.gameObject.SetActive(false);
                    }

                    if (equipmentDatabase.legwear.graphicNamesToHide.Contains(t.gameObject.name))
                    {
                        t.gameObject.SetActive(true);
                    }
                }
            }

            equipmentDatabase.UnequipLegwear();
            ReloadEquipmentGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        #region Accessories
        public bool CanEquipMoreAccessories()
        {
            return BASE_NUMBER_OF_ACCESSORIES_THAT_CAN_EQUIP + GetExtraAccessorySlots() <= Player.instance.equippedAccessories.Count;
        }

        public int GetExtraAccessorySlots()
        {
            int extraSlotsCount = 0;

            foreach (var extraAccessorySlot in extraAccessorySlots)
            {
                bool conditionsMet = true;

                foreach (var requiredSwitch in extraAccessorySlot.requiredSwitches)
                {
                    if (!SwitchManager.instance.GetSwitchCurrentValue(requiredSwitch))
                    {
                        conditionsMet = false;
                        break;
                    }
                }

                if (conditionsMet)
                {
                    extraSlotsCount++;
                }
            }

            return extraSlotsCount;
        }

        public void EquipAccessory(Accessory accessoryToEquip, int slotIndex)
        {
            if (accessoryToEquip == null)
            {
                return;
            }

            equipmentDatabase.EquipAccessory(accessoryToEquip, slotIndex);

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        public void UnequipAccessory(int slotIndex)
        {
            equipmentDatabase.UnequipAccessory(slotIndex);

            statsBonusController.RecalculateEquipmentBonus();
        }

        void ReloadEquipmentGraphics()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                // HELMET
                var helmet = equipmentDatabase.helmet;
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
                var chest = equipmentDatabase.armor;
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
                var gauntlets = equipmentDatabase.gauntlet;
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
                var legwear = equipmentDatabase.legwear;
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


        public void ShakeCamera()
        {
            impulseSource.GenerateImpulse();
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
            else
            {
                rightUnarmedHitbox.EnableHitbox();
            }
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void HitEnd()
        {
            DeactivateAllHitboxes();
        }

        public void ActivateWeaponSpecial()
        {
            /*if (equipmentDatabase.currentWeapon?.weaponSpecial == null)
            {
                return;
            }

            var targetTransform = transform.position + transform.up;

            if (equipmentDatabase.currentWeapon?.instatiateOnGround == true)
            {
                targetTransform.y = transform.position.y;
            }
            var instance = Instantiate(Player.instance.equippedWeapon.weaponSpecial, targetTransform, Quaternion.identity);

            var lookDir = transform.position + transform.forward - instance.transform.position;
            lookDir.y = 0;
            instance.transform.rotation = Quaternion.LookRotation(lookDir);


            if (Player.instance.equippedWeapon.parentWeaponSpecialToPlayer)
            {
                instance.transform.parent = this.transform;
            }*/
        }

        public void EnableCharacterRotation()
        {
            thirdPersonController.canRotateCharacter = true;

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

            if (this.leftUnarmedHitbox != null)
            {
                this.leftUnarmedHitbox.DisableHitbox();
            }
            if (this.rightUnarmedHitbox != null)
            {
                this.rightUnarmedHitbox.DisableHitbox();
            }
        }

        public void ShowShield()
        {
            if (this.shieldGraphic != null)
            {
                this.shieldGraphic.gameObject.SetActive(true);
            }
        }

        public void HideShield()
        {
            /*
            if (this.shieldGraphic == null)
            {
                return;
            }

            if (Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.hideShield == false)
            {
                return;
            }


            this.shieldGraphic.gameObject.SetActive(false);*/
        }
        public void ForceHideShield()
        {
            if (this.shieldGraphic == null)
            {
                return;
            }

            this.shieldGraphic.gameObject.SetActive(false);
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


        #region Equipment Modifiers


        // Every 5 levels, grant 1 extra spell
        public int CalculateExtraSpellUsage()
        {
            return playerStatsDatabase.intelligence + statsBonusController.intelligenceBonus / 5;
        }


        #endregion

        public float GetHeavyWeightThreshold()
        {

            return 0.135f + GetStrengthWeightLoadBonus();
        }

        public float GetMidWeightThreshold()
        {

            return 0.05f + GetStrengthWeightLoadBonus();
        }

        float GetStrengthWeightLoadBonus()
        {
            float bonus = playerStatsDatabase.strength + statsBonusController.strengthBonus;

            bonus *= 0.0025f;

            if (bonus > 0f)
            {
                return bonus;
            }

            return 0f;
        }

        public bool IsLightWeight()
        {
            return statsBonusController.weightPenalty <= GetMidWeightThreshold();
        }

        public bool IsMidWeight()
        {
            return statsBonusController.weightPenalty < GetHeavyWeightThreshold() && statsBonusController.weightPenalty > GetMidWeightThreshold();
        }

        public bool IsHeavyWeight()
        {
            return statsBonusController.weightPenalty >= GetHeavyWeightThreshold();
        }

        public bool IsLightWeightForGivenValue(float givenWeightPenalty)
        {
            return givenWeightPenalty <= GetMidWeightThreshold();
        }

        public bool IsMidWeightForGivenValue(float givenWeightPenalty)
        {
            return givenWeightPenalty < GetHeavyWeightThreshold() && givenWeightPenalty > GetMidWeightThreshold();
        }

        public bool IsHeavyWeightForGivenValue(float givenWeightPenalty)
        {
            return givenWeightPenalty >= GetHeavyWeightThreshold();
        }

        public float GetEquipLoad()
        {
            return statsBonusController.weightPenalty;
        }


    }
}
