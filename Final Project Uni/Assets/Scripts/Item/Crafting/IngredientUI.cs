using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image Icon;
    public TMP_Text NameText;

    public void SetIngredient(Sprite icon, string name)
    {
        Icon.sprite = icon;
        NameText.text = name;
    }
}