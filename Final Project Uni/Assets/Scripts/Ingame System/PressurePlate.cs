using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum InteractTarget
{
    Player, Arrow, Enemy
}

public class PressurePlate : MonoBehaviour
{
    // List of targets to check against
    public List<InteractTarget> validTargets = new List<InteractTarget>();

    public UnityEvent ToggleOn, ToggleOff;
    public float DelayActivateTime, DelayLeftTime;
    public bool pressing;

    // List to track objects currently inside the trigger
    private List<Collider> objectsInTrigger = new List<Collider>();

    // Function to check if the tag matches any in the validTargets list
    private bool IsValidTarget(Collider other)
    {
        // If no specific target is provided, we assume all tags are valid
        if (validTargets.Count == 0) return true;

        // Check if the tag matches any of the valid targets
        foreach (InteractTarget target in validTargets)
        {
            if (other.CompareTag(target.ToString())) return true;
        }

        return false; // No matching tag found
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsValidTarget(other)) return;

        TriggerToggle(other, false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidTarget(other)) return;

        TriggerToggle(other, true);
    }

    async UniTaskVoid TriggerToggle(Collider other, bool toggle)
    {
        if (toggle && objectsInTrigger.Contains(other))
        {
            objectsInTrigger.Remove(other);
            pressing = toggle;
            await UniTask.Delay(TimeSpan.FromSeconds(DelayActivateTime));
            ToggleOff.Invoke();
        }
        else if (!toggle && !objectsInTrigger.Contains(other))
        {
            objectsInTrigger.Add(other);
            pressing = true;
            await UniTask.Delay(TimeSpan.FromSeconds(DelayLeftTime));
            ToggleOn.Invoke();
        }
        
    }

    private void Update()
    {
        // Iterate over objects in the trigger to check if any are disabled
        for (int i = objectsInTrigger.Count - 1; i >= 0; i--)
        {
            if (objectsInTrigger[i] == null || !objectsInTrigger[i].gameObject.activeInHierarchy)
            {
                // If object is disabled or destroyed, invoke ToggleOff and remove it from the list
                objectsInTrigger.RemoveAt(i);
                pressing = false;
                ToggleOff.Invoke();
            }
        }
    }
}
