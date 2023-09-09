using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using static AF.Player;
using Cinemachine;

namespace AF
{


    public class EquipmentGraphicsHandler : MonoBehaviour, ISaveable
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

        StarterAssets.ThirdPersonController thirdPersonController => GetComponent<StarterAssets.ThirdPersonController>();
        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();

        LockOnManager lockOnManager;

        [Header("Equipment Modifiers")]
        public float weightPenalty = 0f;
        public int equipmentPoise = 0;
        public float equipmentPhysicalDefense = 0;
        public List<ArmorBase.StatusEffectResistance> statusEffectResistances = new List<ArmorBase.StatusEffectResistance>();
        
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;

        public float fireDefenseBonus = 0;
        public float frostDefenseBonus = 0;
        public float lightningDefenseBonus = 0;
        public float magicDefenseBonus = 0;

        public float additionalCoinPercentage = 0;

        public float parryPostureDamageBonus = 0;

        public int reputationBonus = 0;

        public float chanceToStealBonus = 0;

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

        RuntimeAnimatorController playerDefaultAnimator;

        PlayerInventory playerInventory => GetComponent<PlayerInventory>();
        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
        }

        void Start()
        {
            bow.gameObject.SetActive(false);

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

            if (Player.instance.equippedAccessories.Count > 0)
            {
                foreach (var acc in Player.instance.equippedAccessories)
                {
                    EquipAccessory(acc);
                }
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
                    if (weaponToEquip.isDualWielded)
                    {
                        var dualWieldBackPivot = weaponToEquip.graphic.GetComponentInChildren<CustomBackWeaponPivot>(true).gameObject;

                        if (dualWieldBackPivot != null)
                        {
                            leftWeaponGraphicBack = Instantiate(dualWieldBackPivot, backRef);
                            leftWeaponGraphicBack.SetActive(false);
                        }
                    }
                    else // Use Normal Back Ref
                    {
                        leftWeaponGraphicBack = Instantiate(weaponToEquip.graphic, backRef);

                        WeaponPivotHandler weaponPivotHandler = leftWeaponGraphicBack.GetComponentInChildren<WeaponPivotHandler>();
                        if (weaponPivotHandler != null && weaponPivotHandler.useCustomBackRefTransform)
                        {
                            weaponPivotHandler.transform.localPosition = weaponPivotHandler.backPosition;
                            weaponPivotHandler.transform.localRotation = Quaternion.Euler(new Vector3(weaponPivotHandler.backRotationX, weaponPivotHandler.backRotationY, weaponPivotHandler.backRotationZ));
                        }

                        leftWeaponGraphicBack.SetActive(false);
                    }

                }
                else if (weaponToEquip.useHolsterRef)
                {
                    GameObject leftWeaponPivotPrefab = weaponToEquip.graphic.GetComponentInChildren<LeftWeaponPivot>(true)?.gameObject;

                    if (leftWeaponPivotPrefab == null)
                    {
                        leftWeaponPivotPrefab = weaponToEquip.graphic;
                    }

                    leftWeaponGraphicHolster = Instantiate(leftWeaponPivotPrefab, leftHolsterRef);
                    leftWeaponGraphicHolster.SetActive(false);

                    if (weaponToEquip.isDualWielded && leftWeaponGraphicHolster != null)
                    {
                        rightWeaponGraphicHolster = Instantiate(weaponToEquip.graphic.GetComponentInChildren<RightWeaponPivot>(true).gameObject, rightHolsterRef);
                        rightWeaponGraphicHolster.transform.rotation = leftWeaponGraphicHolster.transform.rotation;
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

            if (weaponToEquip.hideShield)
            {
                HideShield();
            }

            AssignWeaponHandlerRefs();

            DeactivateAllHitboxes();

            RecalculateEquipmentBonus();


            GetComponent<Animator>().SetLayerWeight(1, weaponToEquip.blockLayerWeight);
        }

        public void AssignWeaponHandlerRefs()
        {
            if (playerCombatController.leftWeaponHandlerRef != null || playerCombatController.rightWeaponHandlerRef != null) { return; }

            HandRef[] handRefs = GetComponentsInChildren<HandRef>(true);
            foreach (var handRef in handRefs)
            {
                var weaponHandlerRefs = handRef.GetComponentsInChildren<WeaponHandlerRef>(true);

                foreach (var weaponHandlerRef in weaponHandlerRefs)
                {
                    if (weaponHandlerRef != null)
                    {
                        if (weaponHandlerRef.isLeft)
                        {
                            playerCombatController.leftWeaponHandlerRef = weaponHandlerRef;
                        }
                        else
                        {
                            playerCombatController.rightWeaponHandlerRef = weaponHandlerRef;
                        }
                    }
                }
            }
        }

        public void UnassignWeaponHandlerRefs()
        {
            playerCombatController.leftWeaponHandlerRef = null;
            playerCombatController.rightWeaponHandlerRef = null;
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
            
            if (Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.hideShield)
            {
                shieldGraphic.SetActive(false);
            }

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
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

            RecalculateEquipmentBonus();
        }
        #endregion

        #region Accessories

        public bool OnUnequipAccessoryCheckIfAccessoryWasDestroyedPermanently(Accessory accessory)
        {
            var unequipedAccessoryIndex = Player.instance.GetEquippedAccessoryIndex(accessory);
            if (unequipedAccessoryIndex == -1)
            {
                return false;
            }

            UnequipAccessory(accessory);

            bool wasDestroyed = CheckIfAccessoryShouldBeDestroyed(accessory);
            return wasDestroyed;
        }

        bool CheckIfAccessoryShouldBeDestroyed(Accessory unequipedAccessory)
        {
            if (unequipedAccessory != null && unequipedAccessory.destroyOnUnequip)
            {

                var itemIdx = Player.instance.ownedItems.FindIndex(x => x.item.name.GetEnglishText() == unequipedAccessory.name.GetEnglishText());

                if (itemIdx != -1)
                {
                    Player.instance.ownedItems.RemoveAt(itemIdx);
                }

                notificationManager.ShowNotification(unequipedAccessory.name.GetText() + " " + LocalizedTerms.WasDestroyedByUnequiping() + ".", notificationManager.systemError);

                if (unequipedAccessory.onUnequipDestroySoundclip != null)
                {
                    BGMManager.instance.PlaySound(unequipedAccessory.onUnequipDestroySoundclip, null);
                }
                SaveSystem.instance.SaveGameData("item_destroyed");

                return true;
            }

            return false;
        }

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

        public void EquipAccessory(Accessory accessoryToEquip)
        {
            if (accessoryToEquip == null)
            {
                return;
            }

            var idx = Player.instance.GetEquippedAccessoryIndex(accessoryToEquip);

            // If accessory is equiped, return;
            if (idx != -1)
            {
                return;
            }

            Player.instance.equippedAccessories.Add(accessoryToEquip);

            RecalculateEquipmentBonus();
        }
#endregion

        public void UnequipWeapon()
        {
            UnassignWeaponHandlerRefs();

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

            GetComponent<Animator>().SetLayerWeight(1, 1f);

            RecalculateEquipmentBonus();
        }

        public void UnequipShield()
        {
            Destroy(shieldGraphic);

            Player.instance.equippedShield = null;

            RecalculateEquipmentBonus();
        }

        public void UnequipAccessory(Accessory accessoryToUnequip)
        {
            var idx = Player.instance.GetEquippedAccessoryIndex(accessoryToUnequip);

            if (idx == -1)
            {
                return;
            }

            Player.instance.equippedAccessories.RemoveAt(idx);
            
            RecalculateEquipmentBonus();
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
            if (Player.instance.equippedWeapon?.weaponSpecial == null)
            {
                return;
            }

            var instance = Instantiate(Player.instance.equippedWeapon.weaponSpecial, transform.position + transform.up, Quaternion.identity);

            var lookDir = transform.position + transform.forward - instance.transform.position;
            lookDir.y = 0;
            instance.transform.rotation = Quaternion.LookRotation(lookDir);

            if (Player.instance.equippedWeapon.parentWeaponSpecialToPlayer)
            {
                instance.transform.parent = this.transform;
            }
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
            if (this.shieldGraphic == null)
            {
                return;
            }

            if (Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.hideShield == false)
            {
                return;
            }


            this.shieldGraphic.gameObject.SetActive(false);
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

        #region Serialization
        public void OnGameLoaded(GameData gameData)
        {
            PlayerEquipmentData playerEquipmentData = gameData.playerEquipmentData;

            if (!String.IsNullOrEmpty(playerEquipmentData.weaponName))
            {
                var weaponFileName = playerEquipmentData.weaponName;

                var idx = playerEquipmentData.weaponName.IndexOf(" +");
                if (idx != -1)
                {
                    weaponFileName = weaponFileName.Substring(0, idx);
                }

                var weapon = Resources.Load<Weapon>("Items/Weapons/" + weaponFileName);

                var equippedWeapon = Instantiate(weapon);
                if (equippedWeapon != null)
                {
                    if (idx != -1)
                    {
                        var weaponLevel = playerEquipmentData.weaponName.Substring(playerEquipmentData.weaponName.IndexOf("+"));

                        int.TryParse(weaponLevel.Replace("+", "").Trim(), out int result);

                        if (result > 0)
                        {
                            equippedWeapon.level = result;
                        }
                    }
                }

                EquipWeapon(equippedWeapon);
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

            if (playerEquipmentData.acessoryNames.Length > 0)
            {
                foreach (var acc in playerEquipmentData.acessoryNames)
                {
                    var accessory = Resources.Load<Accessory>("Items/Accessories/" + acc);
                    EquipAccessory(accessory);
                }
            }
            else
            {
                var originalList = Player.instance.equippedAccessories.ToList();
                foreach (var equippedAcc in originalList)
                {
                    UnequipAccessory(equippedAcc);
                }
            }

        }

        #endregion

        #region Equipment Modifiers
        public void RecalculateEquipmentBonus()
        {
            UpdateWeightPenalty();
            UpdateArmorPoise();
            UpdateEquipmentPhysicalDefense();
            UpdateStatusEffectResistances();
            UpdateAttributes();
            UpdateAdditionalCoinPercentage();
        }

        void UpdateWeightPenalty()
        {
            this.weightPenalty = 0f;

            if (Player.instance.equippedWeapon != null)
            {
                this.weightPenalty += Player.instance.equippedWeapon.speedPenalty;
            }
            if (Player.instance.equippedShield != null)
            {
                this.weightPenalty += Player.instance.equippedShield.speedPenalty;
            }
            if (Player.instance.equippedHelmet != null)
            {
                this.weightPenalty += Player.instance.equippedHelmet.speedPenalty;
            }
            if (Player.instance.equippedArmor != null)
            {
                this.weightPenalty += Player.instance.equippedArmor.speedPenalty;
            }
            if (Player.instance.equippedGauntlets != null)
            {
                this.weightPenalty += Player.instance.equippedGauntlets.speedPenalty;
            }
            if (Player.instance.equippedLegwear != null)
            {
                this.weightPenalty += Player.instance.equippedLegwear.speedPenalty;
            }
            if (Player.instance.equippedAccessories.Count > 0)
            {
                float speedPenaltyAccessories = Player.instance.equippedAccessories.Sum(x => x.speedPenalty);
                this.weightPenalty += speedPenaltyAccessories;
            }

            // Offset (in the case where an item removes weight penalties, like a ring of havel or something
            if (this.weightPenalty < 0)
            {
                this.weightPenalty = 0;
            }
        }

        void UpdateArmorPoise()
        {
            this.equipmentPoise = 0;

            if (Player.instance.equippedHelmet != null)
            {
                equipmentPoise += Player.instance.equippedHelmet.poiseBonus;
            }
            if (Player.instance.equippedArmor != null)
            {
                equipmentPoise += Player.instance.equippedArmor.poiseBonus;
            }
            if (Player.instance.equippedGauntlets != null)
            {
                equipmentPoise += Player.instance.equippedGauntlets.poiseBonus;
            }
            if (Player.instance.equippedLegwear != null)
            {
                equipmentPoise += Player.instance.equippedLegwear.poiseBonus;
            }
            if (Player.instance.equippedAccessories.Count > 0)
            {
                float poiseBonusAccessories = Player.instance.equippedAccessories.Sum(x => x.poiseBonus);
                equipmentPoise += (int)poiseBonusAccessories;
            }
        }

        void UpdateEquipmentPhysicalDefense()
        {
            this.equipmentPhysicalDefense = 0f;

            var player = Player.instance;

            if (player.equippedHelmet != null)
            {
                equipmentPhysicalDefense += player.equippedHelmet.physicalDefense;
            }

            if (player.equippedArmor != null)
            {
                equipmentPhysicalDefense += player.equippedArmor.physicalDefense;
            }

            if (player.equippedGauntlets != null)
            {
                equipmentPhysicalDefense += player.equippedGauntlets.physicalDefense;
            }

            if (player.equippedLegwear != null)
            {
                equipmentPhysicalDefense += player.equippedLegwear.physicalDefense;
            }

            if (Player.instance.equippedAccessories.Count > 0)
            {
                float physicalDefenseBonus = Player.instance.equippedAccessories.Sum(x => x.physicalDefense);
                equipmentPhysicalDefense += physicalDefenseBonus;
            }
        }

        void UpdateStatusEffectResistances()
        {
            this.statusEffectResistances.Clear();

            if (Player.instance.equippedHelmet != null && Player.instance.equippedHelmet.statusEffectResistances.Length > 0)
            {
                foreach (var statusEffectResistance in Player.instance.equippedHelmet.statusEffectResistances)
                {
                    HandleStatusEffectEntry(statusEffectResistance);
                }
            }

            if (Player.instance.equippedArmor != null && Player.instance.equippedArmor.statusEffectResistances.Length > 0)
            {
                foreach (var statusEffectResistance in Player.instance.equippedArmor.statusEffectResistances)
                {
                    HandleStatusEffectEntry(statusEffectResistance);
                }
            }

            if (Player.instance.equippedGauntlets != null && Player.instance.equippedGauntlets.statusEffectResistances.Length > 0)
            {
                foreach (var statusEffectResistance in Player.instance.equippedGauntlets.statusEffectResistances)
                {
                    HandleStatusEffectEntry(statusEffectResistance);
                }
            }

            if (Player.instance.equippedLegwear != null && Player.instance.equippedLegwear.statusEffectResistances.Length > 0)
            {
                foreach (var statusEffectResistance in Player.instance.equippedLegwear.statusEffectResistances)
                {
                    HandleStatusEffectEntry(statusEffectResistance);
                }
            }

            if (Player.instance.equippedAccessories.Count > 0)
            {
                var statusEffectResistances = Player.instance.equippedAccessories.SelectMany(x => x.statusEffectResistances);

                foreach (var statusEffectResistance in statusEffectResistances)
                {
                    HandleStatusEffectEntry(statusEffectResistance);
                }
            }
        }

        void HandleStatusEffectEntry(ArmorBase.StatusEffectResistance statusEffectResistance)
        {
            var clone = new ArmorBase.StatusEffectResistance()
            {
                resistanceBonus = statusEffectResistance.resistanceBonus,
                statusEffect = statusEffectResistance.statusEffect,
            };

            var idx = this.statusEffectResistances.FindIndex(x => x.statusEffect == clone.statusEffect);
            if (idx != -1)
            {
                this.statusEffectResistances[idx].resistanceBonus += clone.resistanceBonus;
            }
            else
            {
                this.statusEffectResistances.Add(clone);
            }
        }

        void UpdateAttributes()
        {
            this.vitalityBonus = 0;
            this.enduranceBonus = 0;
            this.strengthBonus = 0;
            this.dexterityBonus = 0;
            this.intelligenceBonus = 0;

            this.fireDefenseBonus = 0;
            this.frostDefenseBonus = 0;
            this.lightningDefenseBonus = 0;
            this.magicDefenseBonus = 0;

            var initialReputation = 0;
            this.reputationBonus = 0;
            this.parryPostureDamageBonus = 0;

            if (Player.instance.equippedHelmet != null)
            {
                this.vitalityBonus += Player.instance.equippedHelmet.vitalityBonus;
                this.enduranceBonus += Player.instance.equippedHelmet.enduranceBonus;
                this.strengthBonus += Player.instance.equippedHelmet.strengthBonus;
                this.dexterityBonus += Player.instance.equippedHelmet.dexterityBonus;
                this.intelligenceBonus += Player.instance.equippedHelmet.intelligenceBonus;
                this.fireDefenseBonus += Player.instance.equippedHelmet.fireDefense;
                this.frostDefenseBonus += Player.instance.equippedHelmet.frostDefense;
                this.lightningDefenseBonus += Player.instance.equippedHelmet.lightningDefense;
                this.magicDefenseBonus += Player.instance.equippedHelmet.magicDefense;
                this.reputationBonus += Player.instance.equippedHelmet.reputationBonus;
            }
            if (Player.instance.equippedArmor != null)
            {
                this.vitalityBonus += Player.instance.equippedArmor.vitalityBonus;
                this.enduranceBonus += Player.instance.equippedArmor.enduranceBonus;
                this.strengthBonus += Player.instance.equippedArmor.strengthBonus;
                this.dexterityBonus += Player.instance.equippedArmor.dexterityBonus;
                this.intelligenceBonus += Player.instance.equippedArmor.intelligenceBonus;
                this.fireDefenseBonus += Player.instance.equippedArmor.fireDefense;
                this.frostDefenseBonus += Player.instance.equippedArmor.frostDefense;
                this.lightningDefenseBonus += Player.instance.equippedArmor.lightningDefense;
                this.magicDefenseBonus += Player.instance.equippedArmor.magicDefense;
                this.reputationBonus += Player.instance.equippedArmor.reputationBonus;
            }
            if (Player.instance.equippedGauntlets != null)
            {
                this.vitalityBonus += Player.instance.equippedGauntlets.vitalityBonus;
                this.enduranceBonus += Player.instance.equippedGauntlets.enduranceBonus;
                this.strengthBonus += Player.instance.equippedGauntlets.strengthBonus;
                this.dexterityBonus += Player.instance.equippedGauntlets.dexterityBonus;
                this.intelligenceBonus += Player.instance.equippedGauntlets.intelligenceBonus;
                this.fireDefenseBonus += Player.instance.equippedGauntlets.fireDefense;
                this.frostDefenseBonus += Player.instance.equippedGauntlets.frostDefense;
                this.lightningDefenseBonus += Player.instance.equippedGauntlets.lightningDefense;
                this.magicDefenseBonus += Player.instance.equippedGauntlets.magicDefense;
                this.reputationBonus += Player.instance.equippedGauntlets.reputationBonus;
            }
            if (Player.instance.equippedLegwear != null)
            {
                this.vitalityBonus += Player.instance.equippedLegwear.vitalityBonus;
                this.enduranceBonus += Player.instance.equippedLegwear.enduranceBonus;
                this.strengthBonus += Player.instance.equippedLegwear.strengthBonus;
                this.dexterityBonus += Player.instance.equippedLegwear.dexterityBonus;
                this.intelligenceBonus += Player.instance.equippedLegwear.intelligenceBonus;
                this.fireDefenseBonus += Player.instance.equippedLegwear.fireDefense;
                this.frostDefenseBonus += Player.instance.equippedLegwear.frostDefense;
                this.lightningDefenseBonus += Player.instance.equippedLegwear.lightningDefense;
                this.magicDefenseBonus += Player.instance.equippedLegwear.magicDefense;
                this.reputationBonus += Player.instance.equippedLegwear.reputationBonus;
            }
            if (Player.instance.equippedAccessories.Count> 0)
            {
                foreach (var acc in Player.instance.equippedAccessories)
                {
                    this.vitalityBonus += acc.vitalityBonus;
                    this.enduranceBonus += acc.enduranceBonus;
                    this.strengthBonus += acc.strengthBonus;
                    this.dexterityBonus += acc.dexterityBonus;
                    this.intelligenceBonus += acc.intelligenceBonus;
                    this.fireDefenseBonus += acc.fireDefense;
                    this.frostDefenseBonus += acc.frostDefense;
                    this.lightningDefenseBonus += acc.lightningDefense;
                    this.magicDefenseBonus += acc.magicDefense;
                    this.reputationBonus += acc.reputationBonus;
                    this.parryPostureDamageBonus += acc.postureDamagePerParry;
                }
            }

            // Reputation has changed?
            if (initialReputation != this.reputationBonus)
            {
                // Reevaluate behavior
                FactionManager.instance.ReevaluateAllEnemiesInScene();
            }

        }

        void UpdateAdditionalCoinPercentage()
        {
            additionalCoinPercentage = 0f;

            if (Player.instance.equippedHelmet != null)
            {
                additionalCoinPercentage += Player.instance.equippedHelmet.additionalCoinPercentage;
            }
            if (Player.instance.equippedArmor != null)
            {
                additionalCoinPercentage += Player.instance.equippedArmor.additionalCoinPercentage;
            }
            if (Player.instance.equippedGauntlets != null)
            {
                additionalCoinPercentage += Player.instance.equippedGauntlets.additionalCoinPercentage;
            }
            if (Player.instance.equippedLegwear != null)
            {
                additionalCoinPercentage += Player.instance.equippedLegwear.additionalCoinPercentage;
            }
            if (Player.instance.equippedAccessories.Count > 0)
            {
                var additionalCoinPercentageBonuses = Player.instance.equippedAccessories.Sum(x => x.additionalCoinPercentage);
                additionalCoinPercentage += additionalCoinPercentageBonuses;
            }
        }


        // Every 5 levels, grant 1 extra spell
        public int CalculateExtraSpellUsage()
        {
            return Player.instance.intelligence + intelligenceBonus / 5;
        }


        #endregion

    }
}
