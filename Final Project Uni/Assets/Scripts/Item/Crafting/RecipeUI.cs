using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text RecipeNameText;
    public Image RecipeIcon;
    public GameObject UnlockedSprite;
    public GameObject MissingMatSprite; // Indicator if materials are missing
    public Button CraftButton;
    public Transform IngredientsContainer; // Parent for ingredient icons
    public GameObject IngredientPrefab; // Prefab for ingredient display
    public GameObject SplitSpritePrefab; // Prefab for the plus sprite

    private string recipeID;
    private CraftingController craftingTable;

    /// <summary>
    /// Initializes the Recipe UI element with recipe data.
    /// </summary>
    public void SetRecipeUI(CraftingRecipe recipe, bool canCraft, CraftingController table, bool unlocked)
    {
        UnlockedSprite.SetActive(!unlocked);
        if(!unlocked) return;
        

        // Set recipe details
        RecipeNameText.text = recipe.RecipeName;

        // Check if the output GameObject has a SpriteRenderer
        var spriteRenderer = recipe.output.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            RecipeIcon.sprite = spriteRenderer.sprite; // Set the sprite from SpriteRenderer
        else
            Debug.LogWarning($"SpriteRenderer is missing on the output object: {recipe.output.name}");

        recipeID = recipe.RecipeID;
        craftingTable = table;

        // Show or hide the missing material indicator
        MissingMatSprite.SetActive(!canCraft);

        // Set up the Craft button
        CraftButton.onClick.RemoveAllListeners();
        CraftButton.onClick.AddListener(() => craftingTable.Crafting(recipeID));

        // Clear any previous ingredient displays
        foreach (Transform child in IngredientsContainer)
        {
            Destroy(child.gameObject);
        }

        // Display the list of required ingredients
        for (int i = 0; i < recipe.input.Count; i++)
        {
            var ingredient = recipe.input[i];

            // Instantiate ingredient UI
            GameObject ingredientUIObj = Instantiate(IngredientPrefab, IngredientsContainer);
            var ingredientUI = ingredientUIObj.GetComponent<IngredientUI>();
            if (ingredientUI != null)
            {
                // Set the ingredient's icon and name with amount
                string displayName = $"{ingredient.item.itemName} x{ingredient.amount}";
                ingredientUI.SetIngredient(ingredient.item.icon, displayName);
            }

            // Add SplitSprite if this is not the last ingredient
            if (i < recipe.input.Count - 1)
            {
                Instantiate(SplitSpritePrefab, IngredientsContainer);
            }
        }
    }
}
