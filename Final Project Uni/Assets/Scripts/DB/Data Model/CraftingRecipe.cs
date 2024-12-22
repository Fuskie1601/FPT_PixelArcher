using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CraftingRecipe
{
    public string RecipeID;
    public string RecipeName;
    public GameObject output;
    public List<InventoryItem> input;
    public bool defaultUnlocked;
}