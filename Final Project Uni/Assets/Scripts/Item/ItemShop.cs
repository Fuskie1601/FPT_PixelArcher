using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ItemSelling
{
    public GameObject item;
    public bool isSkill;
}

public class ItemShop : MonoBehaviour
{
    [FoldoutGroup("Setup")]
    [SerializeField] private List<GameObject> allShopItems;
    [FoldoutGroup("Setup")]
    public int ItemSellAmount = 3;
    [FoldoutGroup("Setup")]
    [SerializeField] private List<Transform> ItemSellingPlacement;
    [FoldoutGroup("Setup")]
    [SerializeField] private List<TextMeshProUGUI> ItemSellingPrice;
    [FoldoutGroup("Setup")]
    [Range(0,1)] public float SkillPercent = 0.4f;

    [FoldoutGroup("Debug")]
    [ReadOnly] public List<ItemSelling> AvailableShopPool;

    [FoldoutGroup("Debug")] 
    [ReadOnly] public List<ItemSelling> nonSkillItems, skillItems;
    [FoldoutGroup("Debug")] 
    [ReadOnly] public List<GameObject> SellingItems;
    
    private PlayerProgressData _playerProgressData;
    
    private void Start()
    {
        _playerProgressData = PlayerController.Instance.PlayerProgressData;
        //Invoke(nameof(CreatePool), 0.5f);
    }
    
    
    [Button]
    public void CreatePool()
    {
        AvailableShopPool = GenerateItemShopPool();
        SetSellingItems();
    }

    [Button]
    public void SetSellingItems()
    {
        SellingItems.Clear(); // Clear previous items

        // Separate skill and non-skill items
        nonSkillItems = AvailableShopPool.Where(item => !item.isSkill).ToList();
        skillItems = AvailableShopPool.Where(item => item.isSkill).ToList();

        for (int i = 0; i < ItemSellAmount; i++)
        {
            float randomChance = Random.Range(0f, 1f);

            if (skillItems.Count > 0 && randomChance <= SkillPercent)
            {
                int randomSkillIndex = Random.Range(0, skillItems.Count);
                var selectedItem = skillItems[randomSkillIndex];
                SellingItems.Add(selectedItem.item);

                AdjustDuplicateCost(selectedItem.item);
            }
            else if (nonSkillItems.Count > 0)
            {
                int randomNonSkillIndex = Random.Range(0, nonSkillItems.Count);
                var selectedItem = nonSkillItems[randomNonSkillIndex];
                SellingItems.Add(selectedItem.item);

                AdjustDuplicateCost(selectedItem.item);
            }

            // Enable `isItemShop` for each selected item
            foreach (var item in SellingItems)
            {
                var interactableItem = item.GetComponentInChildren<InteractableItem>();
                if (interactableItem != null)
                    interactableItem.isItemShop = true;
            }
        }

        SetItemToPlace();
    }

    // Adjust cost for duplicate items or skills
    private void AdjustDuplicateCost(GameObject item)
    {
        var interactableItem = item.GetComponentInChildren<InteractableItem>();
        if (interactableItem != null)
        {
            int duplicateCount = SellingItems.Count(x => x == item) - 1; // Exclude the first instance
            interactableItem.LastCost = interactableItem.Cost + (int)(interactableItem.Cost * 0.5f * duplicateCount);
            Debug.Log(interactableItem.Cost + " Last Cost: " + interactableItem.LastCost + " | " + duplicateCount);
        }
    }

    void SetItemToPlace()
    {
        // Spawn each item in the SellingItems list at the respective position in ItemSellingPlacement
        for (int i = 0; i < SellingItems.Count && i < ItemSellingPlacement.Count; i++)
        {
            var item = SellingItems[i];
            var placement = ItemSellingPlacement[i];

            // Instantiate or move the item to the placement position
            var spawnedItem = Instantiate(item, placement.position, placement.rotation);
        
            var interactableItem = spawnedItem.GetComponentInChildren<InteractableItem>();
            if (interactableItem != null)
                interactableItem.isItemShop = true;

            if(ItemSellingPrice[i] != null)
                ItemSellingPrice[i].text = interactableItem.Cost.ToString();
        }
    }

    //Calculate
    private List<ItemSelling> GenerateItemShopPool()
    {
        List<ItemSelling> pool = new List<ItemSelling>();

        foreach (var itemObj in allShopItems)
        {
            var item = itemObj.GetComponentInChildren<Item>();
            item.Awake();

            ItemSelling itemSelling = new ItemSelling { item = itemObj, isSkill = item.isSkillBuff};
            
            if (item.isSkillBuff)
            {
                if(item.Skill.GetComponent<ISkill>() == null) Debug.Log(itemObj.name);
                string skillID = item.Skill.GetComponent<ISkill>().Name;

                if (IsSkillUnlockedForItem(skillID))
                    pool.Add(itemSelling);
            }
            else
            {
                pool.Add(itemSelling);
            }
        }

        return pool;
    }

    [Button]
    public void Print(string SkillID)
    {
        Debug.Log( "work ? " + IsSkillUnlockedForItem(SkillID));
    }
    
    private bool IsSkillUnlockedForItem(string skillID)
    {
        // Check in PlayerData if the skill is unlocked
        return _playerProgressData.IsSkillUnlocked(skillID);
    }
}
