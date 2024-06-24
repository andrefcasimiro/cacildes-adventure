using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AF.Stats;
using TigerForge;
using AF.Events;

namespace AF
{
    public class EquipmentGraphicsHandler : MonoBehaviour
    {

        [Header("Components")]
        public StatsBonusController statsBonusController;

        readonly List<string> _helmetNakedParts = new()
        {
            "HairContainer",
            "HeadContainer",
            "EyebrowContainer",
            "BeardContainer",
        };

        readonly List<string> _armorNakedParts = new()
        {
            "TorsoContainer",
            "UpperRightArmContainer",
            "UpperLeftArmContainer",
        };

        readonly List<string> _gauntletsNakedParts = new()
        {
            "LeftLowerArmContainer",
            "RightLowerArmContainer",
            "LeftHandContainer",
            "RightHandContainer"
        };

        readonly List<string> _legwearNakedParts = new()
        {
            "HipContainer",
            "LeftLegContainer",
            "RightLegContainer"
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
            InitializeEquipment();
        }

        public void InitializeEquipment()
        {
            DrawCharacterGraphics();

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

            DrawCharacterGraphics();

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

            DrawCharacterGraphics();

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

            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();

            EventManager.EmitEvent(EventMessages.ON_BODY_TYPE_CHANGED);
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

            DrawCharacterGraphics();

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

            DrawCharacterGraphics();
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
            DrawCharacterGraphics();
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

            DrawCharacterGraphics();

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
            DrawCharacterGraphics();

            statsBonusController.RecalculateEquipmentBonus();
        }
        #endregion

        #region Accessories


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

        void DrawCharacterGraphics()
        {
            foreach (Transform t in playerEquipmentRoot.GetComponentsInChildren<Transform>(true))
            {
                // HELMET
                var helmet = equipmentDatabase.helmet;
                if (helmet == null)
                {
                    if (_helmetNakedParts.IndexOf(t.gameObject.name) != -1)
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
                    if (_armorNakedParts.IndexOf(t.gameObject.name) != -1)
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
                    if (_gauntletsNakedParts.IndexOf(t.gameObject.name) != -1)
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
                    if (_legwearNakedParts.IndexOf(t.gameObject.name) != -1)
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
