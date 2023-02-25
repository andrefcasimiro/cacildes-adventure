using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    [System.Serializable]
    public class CraftingIngredientEntry
    {
        public CraftingMaterial ingredient;

        public int amount;
    }

    public class CraftingRecipe : ScriptableObject
    {
        public new LocalizedText name;

        public Consumable resultingItem;

        public List<CraftingIngredientEntry> ingredients = new List<CraftingIngredientEntry>();
    }

}
