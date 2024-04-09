using System.Collections.Generic;
using AF;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes Database", menuName = "System/New Recipes Database", order = 0)]
public class RecipesDatabase : ScriptableObject
{
    public List<CraftingRecipe> craftingRecipes = new();



#if UNITY_EDITOR 

    private void OnEnable()
    {
        // No need to populate the list; it's serialized directly
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Clear();
        }
    }
#endif

    public void Clear()
    {
        craftingRecipes.Clear();
    }

    public bool HasRecipe(CraftingRecipe recipe)
    {
        return craftingRecipes.Contains(recipe);
    }

    public void AddCraftingRecipe(CraftingRecipe recipe)
    {
        if (HasRecipe(recipe))
        {
            return;
        }

        craftingRecipes.Add(recipe);
    }
}
