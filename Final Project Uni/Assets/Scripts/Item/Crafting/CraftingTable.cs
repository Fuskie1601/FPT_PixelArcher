using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class CraftingController : MonoBehaviour
{
    [CanBeNull] public Transform CraftedItemPlacement;
    
    public GameObject RecipeUIPrefab;
    public Transform RecipeListContent;

    PlayerProgressData _playerProgressData = null;
    private Vector3 pos;
    
    private void Start()
    {
        _playerProgressData = PlayerController.Instance.PlayerProgressData;
    }
    public void OnEnable()
    {
        UpdateRecipeList();
    }
    
    [Button("Update Recipe")]
    public void UpdateRecipeList()
    {
        if(PlayerController.Instance == null) return;
        _playerProgressData = PlayerController.Instance.PlayerProgressData;
        
        // Clear existing UI elements
        foreach (Transform child in RecipeListContent)
        {
            Destroy(child.gameObject);
        }

        // Get recipes and display each in the UI
        foreach (var recipe in _playerProgressData.unlockedRecipes)
        {
            bool canCraft = CheckIfCanCraft(recipe.Recipe);
            var recipeUIObj = Instantiate(RecipeUIPrefab, RecipeListContent);
            RecipeUI recipeUI = recipeUIObj.GetComponent<RecipeUI>();

            // Initialize the RecipeUI element
            recipeUI.SetRecipeUI(recipe.Recipe, canCraft, this, _playerProgressData.IsRecipeUnlocked(recipe.Recipe.RecipeID));
        }
    }
    
    
    //Hàm Logic
    //kiểm tra túi đồ - công thức đã mở khóa 
    //sau đó trừ bớt trong túi người chơi rồi spawn vật
    [Button]
    public void Crafting(string recipeID)
    {
        if(_playerProgressData == null) _playerProgressData = PlayerController.Instance.PlayerProgressData;
        
        // Find the recipe in the player's unlocked recipes
        var recipe = _playerProgressData.unlockedRecipes.Find(r => r.Recipe.RecipeID == recipeID && r.isUnlocked);
        if (recipe == null)
        {
            Debug.Log("Recipe not found or is locked.");
            return;
        }

        // Dictionary to track missing items and their amounts
        Dictionary<string, int> missingItems = new Dictionary<string, int>();
        
        // Check if the player has the required items and amounts
        foreach (var inputItem in recipe.Recipe.input)
        {
            var inventoryItem = _playerProgressData.Inventory.Find(i => i.item.ID == inputItem.item.ID);

            // If the item is not found or the amount is insufficient
            if (inventoryItem == null || inventoryItem.amount < inputItem.amount)
            {
                int missingAmount = inputItem.amount - (inventoryItem?.amount ?? 0);
                missingItems[inputItem.item.ID] = missingAmount;
            }
        }

        // If there are missing items, show a message with details
        if (missingItems.Count > 0)
        {
            foreach (var missingItem in missingItems)
            {
                Debug.Log($"Missing {missingItem.Key}: need {missingItem.Value} more.");
            }
            return;
        }

        // If all items are available, deduct items from the inventory and spawn the output object
        foreach (var inputItem in recipe.Recipe.input)
        {
            var inventoryItem = _playerProgressData.Inventory.Find(i => i.item.ID == inputItem.item.ID);
            inventoryItem.amount -= inputItem.amount;

            // Remove item if amount reaches zero
            if (inventoryItem.amount <= 0)
                _playerProgressData.Inventory.Remove(inventoryItem);
        }

        // Instantiate the crafted item at the specified location
        pos = transform.position;
        if (CraftedItemPlacement != null) pos = CraftedItemPlacement.position;
        Instantiate(recipe.Recipe.output, pos, Quaternion.identity);

        // Save the updated inventory to maintain persistence
        _playerProgressData.SaveClaimReward();
        
        Debug.Log($"Crafted {recipe.Recipe.output.name} successfully!");
    }
    
    /// Checks if the player has enough materials for a given recipe.
    private bool CheckIfCanCraft(CraftingRecipe recipe)
    {
        foreach (var inputItem in recipe.input)
        {
            var inventoryItem = _playerProgressData.Inventory.Find(i => i.item.ID == inputItem.item.ID);
            if (inventoryItem == null || inventoryItem.amount < inputItem.amount)
            {
                return false;
            }
        }
        return true;
    }
}
