using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleUIElements : MonoBehaviour
{
    // List to hold all the UI elements you want to toggle
    public List<GameObject> uiElements;

    public static ToggleUIElements Instance;
    
    public void Start()
    {
        if (Instance == null) Instance = this;
    }

    // Method to toggle the UI elements
    public void ToggleUI(bool isUIEnabled)
    {
        foreach (GameObject uiElement in uiElements)
        {
            uiElement.SetActive(isUIEnabled);
        }
    }
    
    public void ToggleOnSkip(bool isUIEnabled)
    {
        for (int i = 1; i < uiElements.Count; i++)
        {
            uiElements[i].SetActive(isUIEnabled);
        }
    }
    
    public void ToggleUISkip(bool isUIEnabled, int slot = 0)
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            if(i != slot) uiElements[i].SetActive(isUIEnabled);
        }
    }
}
