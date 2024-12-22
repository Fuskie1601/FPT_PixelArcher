using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct ItemPoolEntry
{
    public GameObject item;
    public bool isSkillItem;
}

public class RandomItem : MonoBehaviour
{
    [FoldoutGroup("Setup")] public List<GameObject> allItems;
    [FoldoutGroup("Debug")] [ReadOnly] public List<ItemPoolEntry> availableItemsPool;
    [FoldoutGroup("Debug")] [ReadOnly] public List<ItemPoolEntry> skillItems, nonSkillItems;

    private PlayerProgressData _playerProgressData;

    private void Start()
    {
        _playerProgressData = PlayerController.Instance.PlayerProgressData; // Access player data for skill checks
        Invoke(nameof(CreateAvailableItemsPool), 0.05f); // Delay to ensure PlayerData is initialized
    }
    
    [Button]
    public void RandomSwap()
    {
        if (availableItemsPool.Count == 0)
        {
            Debug.LogWarning("No available items in the pool.");
            return;
        }

        // Choose a random item from the available items pool
        int randomIndex = Random.Range(0, availableItemsPool.Count);
        GameObject selectedItem = availableItemsPool[randomIndex].item;
        // Save the current transform data
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        Vector3 currentScale = transform.localScale;

        // Instantiate the selected item at the original position, rotation, and scale
        GameObject newItem = Instantiate(selectedItem, currentPosition, currentRotation);
        newItem.transform.localScale = currentScale;
        newItem.GetComponentInChildren<InteractableItem>().isItemShop = false;
        
        Debug.Log(newItem.name);

        // Destroy the current game object
        //Destroy(this.gameObject);

        // Remove the used item from the available pool to prevent duplicates
        availableItemsPool.RemoveAt(randomIndex);
    }

    [Button]
    public void CreateAvailableItemsPool()
    {
        availableItemsPool = GenerateAvailableItemPool();
    }
    private List<ItemPoolEntry> GenerateAvailableItemPool()
    {
        List<ItemPoolEntry> pool = new List<ItemPoolEntry>();

        // Separate items based on skill unlock status
        foreach (var itemObj in allItems)
        {
            var interactableItem = itemObj.GetComponentInChildren<Item>(); // Access Item component

            if (interactableItem == null)
            {
                Debug.LogWarning($"Item {itemObj.name} has no Item component.");
                continue;
            }

            ItemPoolEntry entry = new ItemPoolEntry
            {
                item = itemObj,
                isSkillItem = interactableItem.isSkillBuff
            };
            
            skillItems.Clear();
            nonSkillItems.Clear();

            // Check if the item is skill-based and if the skill is unlocked
            if (interactableItem.isSkillBuff && interactableItem.Skill.GetComponent<ISkill>() != null)
            {
                string skillID = interactableItem.Skill.GetComponent<ISkill>().Name;
                if (IsSkillUnlocked(skillID))
                {
                    skillItems.Add(entry);
                    pool.Add(entry);
                }
            }
            else
            {
                nonSkillItems.Add(entry);
                pool.Add(entry);
            }
        }

        return pool;
    }
    private bool IsSkillUnlocked(string skillID)
    {
        // Check in PlayerData if the skill is unlocked
        return _playerProgressData.IsSkillUnlocked(skillID);
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }

}
