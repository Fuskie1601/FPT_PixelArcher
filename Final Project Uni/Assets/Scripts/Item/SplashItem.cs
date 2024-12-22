using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Sirenix.OdinInspector;
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
public class ChestOpeningEffect : MonoBehaviour
{
    [FoldoutGroup("Chest Settings")] public GameObject chestLid; // Reference to the chest lid
    [FoldoutGroup("Chest Settings")] public Transform spawnPoint; // Where the items will spawn from
    [FoldoutGroup("Chest Settings")] public float lidOpenAngle = 90f; // How much the lid should open
    [FoldoutGroup("Chest Settings"), CanBeNull] public Animator chestAnim; // How much the lid should open

    [FoldoutGroup("Burst Settings")] public float minBurstForce = 3f; // Minimum force applied to items
    [FoldoutGroup("Burst Settings")] public float maxBurstForce = 7f; // Maximum force applied to items
    [FoldoutGroup("Burst Settings")] public float burstRadius = 1f; // Radius of the burst effect
    [FoldoutGroup("Burst Settings")] public float minUpForce = 1f; // Minimum upward force applied to items
    [FoldoutGroup("Burst Settings")] public float maxUpForce = 3f; // Maximum upward force applied to items

    [FoldoutGroup("Rarity")] public Rarity rarity;
    [FoldoutGroup("Rarity")] public bool isRandomized = false;
    [FoldoutGroup("Rarity")] public int minRarity;
    [FoldoutGroup("Rarity")] public int maxRarity;
    [FoldoutGroup("Rarity")][Range(0f, 1f)] public float commonRate;
    [FoldoutGroup("Rarity")][Range(0f, 1f)] public float uncommonRate;
    [FoldoutGroup("Rarity")][Range(0f, 1f)] public float rareRate;
    [FoldoutGroup("Rarity")][Range(0f, 1f)] public float epicRate;
    [FoldoutGroup("Rarity")][Range(0f, 1f)] public float legendaryRate;

    [FoldoutGroup("Items")] public GameObject itemToSpawn; // List of items to burst out
    [FoldoutGroup("Items")] public List<GameObject> items = new List<GameObject>(); // List of items to burst out

    [FoldoutGroup("Coins")] public GameObject coinPrefabSmall; // Prefab for the first type of coin
    [FoldoutGroup("Coins")] public GameObject coinPrefabMedium; // Prefab for the second type of coin
    [FoldoutGroup("Coins")] public GameObject coinPrefabBig; // Prefab for the third type of coin
    [FoldoutGroup("Coins")] public int minCoins = 5; // Minimum number of coins to spawn
    [FoldoutGroup("Coins")] public int maxCoins = 10; // Maximum number of coins to spawn
    [FoldoutGroup("Coins")] public bool isFixedCoinRate = false; 
    [FoldoutGroup("Coins")][Range(0f, 1f)] public float coinType1Rate; // Spawn rate for coin type 1
    [FoldoutGroup("Coins")][Range(0f, 1f)] public float coinType2Rate; // Spawn rate for coin type 2
    [FoldoutGroup("Coins")][Range(0f, 1f)] public float coinType3Rate; // Spawn rate for coin type 3





    private bool isOpened = false;
    private void Start()
    {
        SetupRarity();
    }
    void OpenChest()
    {
        chestLid.transform.Rotate(Vector3.right, lidOpenAngle);
    }

    public void BurstItems()
    {
        if (isOpened) return; 
        
        if(chestAnim != null) chestAnim.SetTrigger("Open");
        
        if (items.Count > 0) 
        {
            foreach (GameObject item in items)
            {
                GameObject newItem = Instantiate(item, spawnPoint.position, Quaternion.identity);
                newItem.SetActive(true);
                Rigidbody rb = newItem.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    float randomBurstForce = Random.Range(minBurstForce, maxBurstForce);
                    float randomUpForce = Random.Range(minUpForce, maxUpForce);
                    Vector3 force = Random.insideUnitSphere * randomBurstForce + Vector3.up * randomUpForce;
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }
        }
        
        BurstCoins();
        isOpened = true;
    }

    private void BurstCoins()
    {
        int coinCount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinPrefab = ChooseCoinPrefab();
            GameObject newCoin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
            newCoin.SetActive(true);
            Rigidbody rb = newCoin.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float randomBurstForce = Random.Range(minBurstForce, maxBurstForce);
                float randomUpForce = Random.Range(minUpForce, maxUpForce);
                Vector3 force = Random.insideUnitSphere * randomBurstForce + Vector3.up * randomUpForce;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    private GameObject ChooseCoinPrefab()
    {
        float randomValue = Random.value;

        if (randomValue < coinType1Rate)
        {
            //Debug.Log("spawn coin small with: " + randomValue * 100 + "%");
            return coinPrefabSmall;
        }
        else if (randomValue < coinType1Rate + coinType2Rate)
        {
            //Debug.Log("spawn coin medium with: " + randomValue * 100 + "%");
            return coinPrefabMedium;

        }
        else
        {
            //Debug.Log("spawn coin big with: " + randomValue * 100 + "%");
            return coinPrefabBig;

        }
    }

    private void SetupRarity()
    {
        if (isRandomized)
        {
            float randomValue = Random.value;
            if (randomValue < commonRate)
            {
                rarity = Rarity.Common;
            }
            else if (randomValue < commonRate + uncommonRate)
            {
                rarity = (Rarity.Uncommon);
            }
            else if (randomValue < commonRate + uncommonRate + rareRate)
            {
                rarity = (Rarity.Rare);
            }
            else if (randomValue < commonRate + uncommonRate + rareRate + epicRate)
            {
                rarity = (Rarity.Epic);
            }
            else
            {
                rarity = (Rarity.Legendary);
            }

            if (rarity > (Rarity)maxRarity)
            {
                rarity = (Rarity)maxRarity;
            }
        }

        if (isFixedCoinRate) return;
        switch (rarity)
        {
            case Rarity.Common:
                coinType1Rate = 0.5f;
                coinType2Rate = 0.3f;
                coinType3Rate = 0.2f;
                break;
            case Rarity.Uncommon:
                coinType1Rate = 0.4f;
                coinType2Rate = 0.3f;
                coinType3Rate = 0.3f;
                break;
            case Rarity.Rare:
                coinType1Rate = 0.3f;
                coinType2Rate = 0.3f;
                coinType3Rate = 0.4f;
                items.Add(itemToSpawn);
                break;
            case Rarity.Epic:
                coinType1Rate = 0.2f;
                coinType2Rate = 0.2f;
                coinType3Rate = 0.6f;
                items.Add(itemToSpawn);
                break;
            case Rarity.Legendary:
                coinType1Rate = 0.1f;
                coinType2Rate = 0.2f;
                coinType3Rate = 0.7f;
                items.Add(itemToSpawn);
                items.Add(itemToSpawn);
                break;
        }
    }
}



