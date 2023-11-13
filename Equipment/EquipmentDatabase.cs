using AF;
using UnityEditor;
using UnityEngine;
using TigerForge;
using AF.Events;

[CreateAssetMenu(fileName = "Equipment Database", menuName = "System/New Equipment Database", order = 0)]
public class EquipmentDatabase : ScriptableObject
{
    [Header("Offensive Gear")]
    public Weapon[] weapons = new Weapon[3]; // Fixed size array for weapons

    public Shield[] shields = new Shield[3]; // Fixed size array for shields

    // Arrows    public List<Arrows> arrows;
    public Spell[] spells = new Spell[5];

    public Consumable[] consumables = new Consumable[5];


    [Header("Defensive Gear")]
    public Helmet helmet;
    public Armor armor;
    public Gauntlet gauntlet;
    public Legwear legwear;

    [Header("Accessories")]
    public Accessory[] accessories = new Accessory[2];

    public int currentWeaponIndex, currentShieldIndex, currentConsumableIndex, currentSpellIndex;

    private void OnEnable()
    {
        // No need to populate the list; it's serialized directly
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Clear the list when exiting play mode
            Clear();
        }
    }

    public void Clear()
    {
        currentWeaponIndex = 0;

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

        EventManager.EmitEvent(EventMessages.ON_WEAPON_CHANGED);
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

    public void EquipShield(Shield shield, int slotIndex)
    {
        shields[slotIndex] = shield;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }
    public void UnequipShield(int slotIndex)
    {
        shields[slotIndex] = null;
        EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
    }

    public void SwitchToNextShield()
    {
        currentShieldIndex++;

        if (currentShieldIndex >= shields.Length)
        {
            currentShieldIndex = 0;
        }
    }

    /*
    public void EquipArrows(Arrows arrow) => arrows.Add(arrow);
    public void UnequipArrows(Arrows arrow) => arrows.Remove(arrow);
    */

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

    public void SwitchToNextSpell()
    {
        currentSpellIndex++;

        if (currentSpellIndex >= spells.Length)
        {
            currentSpellIndex = 0;
        }
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

    public void SwitchToNextConsumable()
    {
        currentConsumableIndex++;

        if (currentConsumableIndex >= consumables.Length)
        {
            currentConsumableIndex = 0;
        }
    }

    public void EquipHelmet(Helmet equip) => helmet = equip;
    public void UnequipHelmet() => helmet = null;

    public void EquipArmor(Armor equip) => armor = equip;
    public void UnequipArmor() => armor = null;

    public void EquipGauntlet(Gauntlet equip) => gauntlet = equip;
    public void UnequipGauntlet() => gauntlet = null;

    public void EquipLegwear(Legwear equip) => legwear = equip;
    public void UnequipLegwear() => legwear = null;

    public void EquipAccessory(Accessory accessory, int slotIndex) => accessories[slotIndex] = accessory;
    public void UnequipAccessory(int slotIndex) => accessories[slotIndex] = null;

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
    public Consumable GetCurrentConsumable()
    {
        return consumables[currentConsumableIndex];
    }

}
