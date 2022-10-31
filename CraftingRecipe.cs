using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    [System.Serializable]
    public class CraftingIngredientEntry
    {
        public AlchemyIngredient ingredient;

        public int amount;
    }

    public class CraftingRecipe : ScriptableObject
    {
        public Consumable resultingItem;

        public List<CraftingIngredientEntry> ingredients = new List<CraftingIngredientEntry>();
    }

}
