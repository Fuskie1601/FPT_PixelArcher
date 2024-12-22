using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "ScriptableObjects/RecipeDatabase", order = 1)]
public class RecipeDatabase : ScriptableObject
{
    public List<CraftingRecipe> AllRecipes;
}