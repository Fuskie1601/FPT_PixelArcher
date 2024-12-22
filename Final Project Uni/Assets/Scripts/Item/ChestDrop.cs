using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestDrop : MonoBehaviour
{
    public UnityEvent onChestLanded;

    private bool hasLanded = false;
    public float delayTime = 2f;
    void OnCollisionEnter(Collision collision)
    {
        // Check if the chest has landed
        if (!hasLanded && collision.gameObject.CompareTag("Untagged"))
        {
            hasLanded = true;
            onChestLanded.Invoke();
        }
    }
}
