using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class MeshColliderToggleWithSkip : MonoBehaviour
{
    // A flag to track the current toggle state
    private bool areCollidersEnabled = true;

    [Tooltip("List of GameObjects to skip when toggling MeshColliders.")]
    public List<GameObject> objectsToSkip;

    [Button("Toggle Mesh Colliders")]
    public void ToggleMeshColliders()
    {
        // Get all MeshColliders in the children of this GameObject
        MeshCollider[] childColliders = GetComponentsInChildren<MeshCollider>();

        foreach (MeshCollider collider in childColliders)
        {
            // Check if the parent GameObject of this MeshCollider is in the skip list
            if (ShouldSkip(collider.gameObject))
            {
                continue;
            }

            // Toggle the enabled state of the MeshCollider
            collider.enabled = !areCollidersEnabled;
        }

        // Update the toggle state
        areCollidersEnabled = !areCollidersEnabled;
    }

    // Helper method to check if a GameObject or its parent is in the skip list
    private bool ShouldSkip(GameObject obj)
    {
        // Check if the object itself or any of its parents are in the skip list
        foreach (GameObject skipObject in objectsToSkip)
        {
            if (obj == skipObject || obj.transform.IsChildOf(skipObject.transform))
            {
                return true;
            }
        }
        return false;
    }
}