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

        public Item resultingItem;

        public List<CraftingIngredientEntry> ingredients = new();

        [TextAreaAttribute(minLines: 1, maxLines: 2)] public string location;
    }
}
