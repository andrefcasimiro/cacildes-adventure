using System.Collections.Generic;
using AF;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes Database", menuName = "System/New Recipes Database", order = 0)]
public class RecipesDatabase : ScriptableObject
{
    public List<AlchemyRecipe> alchemyRecipes = new();

    public List<CookingRecipe> cookingRecipes = new();

    [Header("Settings")]
    public bool shouldClearOnExitPlayMode = false;

    private void OnEnable()
    {
        // No need to populate the list; it's serialized directly
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode && shouldClearOnExitPlayMode)
        {
            Clear();
        }
    }

    void Clear()
    {
        alchemyRecipes.Clear();
        cookingRecipes.Clear();
    }

    public void AddAlchemyRecipe(AlchemyRecipe recipe)
    {
        alchemyRecipes.Add(recipe);
    }
    public void AddCookingRecipe(CookingRecipe recipe)
    {
        cookingRecipes.Add(recipe);
    }
}
