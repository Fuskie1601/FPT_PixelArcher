using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableItem : MonoBehaviour
{
    [CanBeNull] public GameObject ItemParent;
    public Button interactButton;

    // New boolean to control if UI interaction is one-time only
    public bool HideAfterUseUI = false;
    public bool dontShowUI = false;

    // Unity events for interaction and trigger range handling
    public UnityEvent InteractEvent, EnterTriggerRange, ExitTriggerRange, ShopFail;

    [FoldoutGroup("Shop")]
    public int Cost = 100;
    [FoldoutGroup("Shop")]
    public bool isItemShop = false;


    private bool hasInteracted = false;
    [ReadOnly] public int LastCost;

    private void Start()
    {
        interactButton = PlayerController.Instance.interactButton;
    }

    private void OnDisable()
    {
        hasInteracted = true;  // Mark as interacted
        if (interactButton == null) return;
        interactButton.gameObject.SetActive(false);  // Hide the button after interaction
        interactButton.onClick.RemoveListener(OnInteract);
    }

    // Method to be called when the interact button is clicked
    public void OnInteract()
    {
        if (isItemShop && PlayerController.Instance.PlayerProgressData.Gold <= LastCost)
        {
            ShopFail.Invoke();
            Debug.Log( "nah u poor af");
            return;
        }
        if (HideAfterUseUI && hasInteracted) 
        {
            Debug.Log(gameObject.name + " is stuck");
            return;
        }

        if (isItemShop) 
        {
            Debug.Log("buy cost: " + LastCost);
            PlayerController.Instance.PlayerProgressData.AddCurrency(-LastCost);
        }
        
        
        InteractEvent.Invoke();

        if (HideAfterUseUI)
        {
            hasInteracted = true;  // Mark as interacted
            interactButton.gameObject.SetActive(false);  // Hide the button after interaction
            interactButton.onClick.RemoveListener(OnInteract);
        }
    }

    // Method to show/hide the interact UI
    public void ShowUIInteract(bool toggle)
    {
        if(dontShowUI) return;
        if (!hasInteracted || !HideAfterUseUI)  // Only show the button if interaction hasn't happened (for one-time use)
        {
            interactButton.gameObject.SetActive(toggle);
        }
    }

    public void SelfDestruct()
    {
        if(ItemParent != null) Destroy(ItemParent);
        else Destroy(gameObject);
    }
    
    public void PlayBuffParticle(string particleID)
    {
        ParticleManager.Instance.SpawnParticle(particleID, transform.position, Quaternion.Euler(-90, 0, 0));
    }

    public void TogglePlayerUI(bool toggle)
    {
        ToggleUIElements.Instance.ToggleUI(toggle);
    }

    // Called when a player enters the interaction range
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Add the listener when entering the trigger range
        if (!hasInteracted)
        {
            interactButton.onClick.RemoveListener(OnInteract);
            interactButton.onClick.AddListener(OnInteract);
        }

        EnterTriggerRange.Invoke();
        ShowUIInteract(true);
    }

    // Called when a player leaves the interaction range
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Remove the listener when exiting the trigger range to prevent stacking interactions
        interactButton.onClick.RemoveListener(OnInteract);
        //hasInteracted = false;

        ExitTriggerRange.Invoke();
        ShowUIInteract(false);
    }
}
