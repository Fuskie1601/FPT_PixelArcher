using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public struct ItemDrop
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropRate; // Percentage scale (0 - 100%)
}

public class ItemDropRate : MonoBehaviour
{
    public bool DropOneItemOnly;
    public List<ItemDrop> itemDrops;
    
    [FoldoutGroup("Level")]
    public int Level = 0;
    [FoldoutGroup("Soul")]
    public int MinSoul = 0, MaxSoul;
    [FoldoutGroup("Soul")]
    [Range(0, 100)] public float soulDropRate = 100f; // 100% drop by default
    [FoldoutGroup("Gold")]
    public int MinGold = 0, MaxGold;
    [FoldoutGroup("Gold")]
    [Range(0, 100)] public float goldDropRate = 100f; // 100% drop by default
    
    
    // Drop rates for Gold and Soul
    
    // Customizable offset
    public Vector3 FixedOffset = new Vector3(0, 2f, 0); 
    public float range = 2f;

    private int Gold, Soul;

    [Button]
    public void CalculateDrops()
    {
        if (DropOneItemOnly)
            DropOneItem();
        else
            DropMultipleItems();
        
        DropCurrency();
    }

    private void DropOneItem()
    {
        float totalProbability = 0f;
        float randomValue = Random.Range(0f, 100f);

        foreach (var itemDrop in itemDrops)
        {
            totalProbability += itemDrop.dropRate;

            if (randomValue <= totalProbability)
            {
                DropItem(itemDrop.itemPrefab);
                return; // Drop only one item, then exit
            }
        }
    }
    private void DropMultipleItems()
    {
        foreach (var itemDrop in itemDrops)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= itemDrop.dropRate)
            {
                DropItem(itemDrop.itemPrefab);
            }
        }
    }

    private void DropCurrency()
    {
        // Drop Gold based on the drop rate
        if (Random.Range(0f, 100f) <= goldDropRate)
        {
            Gold = Random.Range(MinGold, MaxGold);
            PlayerController.Instance.PlayerProgressData.AddCurrency(Gold, 0); // Add Gold
            Debug.Log($"Dropped Gold: {Gold}");
        }

        // Drop Soul based on the drop rate
        if (Random.Range(0f, 100f) <= soulDropRate)
        {
            Soul = Random.Range(MinSoul, MaxSoul);
            PlayerController.Instance.PlayerProgressData.AddCurrency(0, Soul); // Add Soul
            Debug.Log($"Dropped Soul: {Soul}");
        }
    }

    private void DropItem(GameObject item)
    {
        if (item != null)
        {
            // Generate a random offset with y set to 1
            Vector3 randomOffset = new Vector3(
                Random.Range(-range, range), 
                1f, 
                Random.Range(-range, range)
            );

            // Instantiate item with the combined offset
            Instantiate(item, transform.position + randomOffset + FixedOffset, Quaternion.identity);
            Debug.Log($"Dropped: {item.name}");
        }
    }

    private void LevelUp()
    {
        PlayerController.Instance.PlayerProgressData.KnowledgeLevel += 1;
    }
}