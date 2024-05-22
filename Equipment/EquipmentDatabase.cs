using AF;
using UnityEditor;
using UnityEngine;
using TigerForge;
using AF.Events;
using System.Linq;
using AF.Inventory;
using System;

[CreateAssetMenu(fileName = "Equipment Database", menuName = "System/New Equipment Database", order = 0)]
public class EquipmentDatabase : ScriptableObject
{
    [Header("Offensive Gear")]
    public Weapon[] weapons = new Weapon[3]; // Fixed size array for weapons

    public Shield[] shields = new Shield[3]; // Fixed size array for shields

    public Arrow[] arrows = new Arrow[2];

    public Spell[] spells = new Spell[5];

    public Consumable[] consumables = new Consumable[10];

    [Header("Defensive Gear")]
    public Helmet helmet;
    public Armor armor;
    public Gauntlet gauntlet;
    public Legwear legwear;

    [Header("Accessories")]
    public Accessory[] accessories = new Accessory[4];

    public int currentWeaponIndex, currentShieldIndex, currentConsumableIndex, currentSpellIndex, currentArrowIndex = 0;


    [Header("Flags")]
    public bool isTwoHanding = false;
    public bool isUsingShield = false;

    [Header("Databases")]
    public InventoryDatabase inventoryDatabase;

    public bool shouldClearOnExit = false;

#if UNITY_EDITOR
    private void OnEnable()
    {
        // No need to populate the list; it's serialized directly
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode && shouldClearOnExit)
        {
            // Clear the list when exiting play mode
            Clear();
        }
    }
#endif

    public void Clear()
    {
        currentWeaponIndex = 0;
        currentShieldIndex = 0;
        currentConsumableIndex = 0;
        currentSpellIndex = 0;
        currentArrowIndex = 0;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = null;
        }

        for (int i = 0; i < shields.Length; i++)
        {
            shields[i] = null;
        }

        for (int i = 0; i < spells.Length; i++)
        {
            spells[i] = null;
        }

        for (int i = 0; i < accessories.Length; i++)
        {
            accessories[i] = null;
        }

        for (int i = 0; i < consumables.Length; i++)
        {
            consumables[i] = null;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i] = null;
        }

        helmet = null;
        armor = null;
        gauntlet = null;
        legwear = null;
    }
    public void SwitchToNextWeapon()
    {
        currentWeaponIndex++;

        if (currentWeaponIndex >= weapons.Length)
        {
            currentWeaponIndex = 0;
        }

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipWeapon(Weapon weapon, int slotIndex)
    {
        weapons[slotIndex] = weapon;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }
    public void UnequipWeapon(int slotIndex)
    {
        weapons[slotIndex] = null;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void SwitchToNextShield()
    {
        currentShieldIndex++;

        if (currentShieldIndex >= shields.Length)
        {
            currentShieldIndex = 0;
        }

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        EventManager.EmitEvent(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED);
    }

    public void EquipShield(Shield shield, int slotIndex)
    {
        shields[slotIndex] = shield;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        EventManager.EmitEvent(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED);
    }
    public void UnequipShield(int slotIndex)
    {
        shields[slotIndex] = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        EventManager.EmitEvent(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED);
    }

    public void SwitchToNextArrow()
    {
        currentArrowIndex = UpdateIndex(currentArrowIndex, arrows);

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipArrow(Arrow arrow, int slotIndex)
    {
        arrows[slotIndex] = arrow;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }
    public void UnequipArrow(int slotIndex)
    {
        arrows[slotIndex] = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void SwitchToNextSpell()
    {
        currentSpellIndex = UpdateIndex(currentSpellIndex, spells);

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipSpell(Spell spell, int slotIndex)
    {
        spells[slotIndex] = spell;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }
    public void UnequipSpell(int slotIndex)
    {
        spells[slotIndex] = null;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void SwitchToNextConsumable()
    {
        currentConsumableIndex = UpdateIndex(currentConsumableIndex, consumables);
    }

    public void EquipConsumable(Consumable consumable, int slotIndex)
    {
        consumables[slotIndex] = consumable;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void UnequipConsumable(int slotIndex)
    {
        consumables[slotIndex] = null;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public int UpdateIndex<T>(int index, T[] targetList)
    {
        index++;

        int nextIndexWithEquippedSlot = Array.FindIndex(targetList, index, x => x != null);

        if (nextIndexWithEquippedSlot != -1)
        {
            index = nextIndexWithEquippedSlot;
        }
        else
        {
            int fallbackIndex = Array.FindIndex(targetList, 0, x => x != null);
            index = fallbackIndex == -1 ? 0 : fallbackIndex;
        }

        return index;
    }

    public void EquipHelmet(Helmet equip)
    {
        helmet = equip;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }
    public void UnequipHelmet()
    {
        helmet = null;

        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipArmor(Armor equip)
    {
        armor = equip;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void UnequipArmor()
    {
        armor = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipGauntlet(Gauntlet equip)
    {
        gauntlet = equip;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void UnequipGauntlet()
    {
        gauntlet = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipLegwear(Legwear equip)
    {
        legwear = equip;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void UnequipLegwear()
    {
        legwear = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void EquipAccessory(Accessory accessory, int slotIndex)
    {
        accessories[slotIndex] = accessory;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void UnequipAccessory(int slotIndex)
    {
        accessories[slotIndex] = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons[currentWeaponIndex];
    }
    public Shield GetCurrentShield()
    {
        return shields[currentShieldIndex];
    }
    public Spell GetCurrentSpell()
    {
        return spells[currentSpellIndex];
    }
    public Arrow GetCurrentArrow()
    {
        return arrows[currentArrowIndex];
    }
    public Consumable GetCurrentConsumable()
    {
        return consumables[currentConsumableIndex];
    }

    public bool IsAccessoryEquiped(Accessory accessory)
    {
        return accessories.Contains(accessory);
    }

    public bool IsBowEquipped()
    {
        return GetCurrentWeapon() != null && GetCurrentWeapon().damage.weaponAttackType == WeaponAttackType.Range;
    }

    public bool IsStaffEquipped()
    {
        return GetCurrentWeapon() != null && GetCurrentWeapon().damage.weaponAttackType == WeaponAttackType.Staff;
    }

    public bool HasEnoughCurrentArrows()
    {
        Arrow currentArrow = GetCurrentArrow();

        if (currentArrow == null)
        {
            return false;
        }

        return inventoryDatabase.GetItemAmount(currentArrow) > 0;
    }

    public bool IsPlayerNaked()
    {
        return helmet == null && armor == null && legwear == null && gauntlet == null;
    }

    public int GetEquippedWeaponSlot(Weapon weapon)
    {
        return Array.IndexOf(weapons, weapon);
    }
    public int GetEquippedShieldSlot(Shield shield)
    {
        return Array.IndexOf(shields, shield);
    }
    public int GetEquippedArrowsSlot(Arrow arrow)
    {
        return Array.IndexOf(arrows, arrow);
    }
    public int GetEquippedSpellSlot(Spell spell)
    {
        return Array.IndexOf(spells, spell);
    }
    public int GetEquippedAccessoriesSlot(Accessory accessory)
    {
        return Array.IndexOf(accessories, accessory);
    }
    public int GetEquippedConsumablesSlot(Consumable consumable)
    {
        return Array.IndexOf(consumables, consumable);
    }



    public void UnequipItem(Item item)
    {
        // Check weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == item)
            {
                weapons[i] = null;
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            }
        }

        // Check shields
        for (int i = 0; i < shields.Length; i++)
        {
            if (shields[i] == item)
            {
                shields[i] = null;
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            }
        }

        // Check helmet
        if (helmet == item)
        {
            helmet = null;
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        // Check armor
        if (armor == item)
        {
            armor = null;
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        // Check gauntlet
        if (gauntlet == item)
        {
            gauntlet = null;
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        // Check legwear
        if (legwear == item)
        {
            legwear = null;
            EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
        }

        // Check arrows
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == item)
            {
                arrows[i] = null;
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            }
        }

        // Check spells
        for (int i = 0; i < spells.Length; i++)
        {
            if (spells[i] == item)
            {
                spells[i] = null;
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            }
        }

        // Check accessories
        for (int i = 0; i < accessories.Length; i++)
        {
            if (accessories[i] == item)
            {
                accessories[i] = null;
                EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
            }
        }

        // Item not found equipped
        Debug.LogWarning($"UnequipItem: Item '{item.name}' not found equipped");
    }


    public void SetIsTwoHanding(bool value)
    {
        isTwoHanding = value;

        EventManager.EmitEvent(EventMessages.ON_TWO_HANDING_CHANGED);
    }

}
