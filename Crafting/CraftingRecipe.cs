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
        [Header("!! English name must match filename")]
        public new LocalizedText name;

        public Item resultingItem;

        public List<CraftingIngredientEntry> ingredients = new List<CraftingIngredientEntry>();
    }

}
