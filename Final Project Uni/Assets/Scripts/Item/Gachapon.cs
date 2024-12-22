using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Gachapon : MonoBehaviour
{
    public int Cost = 100;
    public int GachaTime;  // Current gacha count
    public int maxGachaTime = 2;  // Maximum number of times a player can do gacha
    public GameObject gachaPrefab;
    public Transform OutputTransform;

    [FoldoutGroup("Event")]
    public UnityEvent ShopFail, MaxGachaReached;

    public void doGacha()
    {
        // Check if player has enough resources and has not exceeded max gacha time
        if (GachaTime >= maxGachaTime)
        {
            Debug.Log("Max gacha time reached.");
            MaxGachaReached.Invoke();  // Trigger event if max gacha time reached
            return;
        }

        if (PlayerController.Instance._stats.playerSoul < Cost)
        {
            ShopFail.Invoke();
            return;
        }

        // Deduct the cost and increment gacha count
        PlayerController.Instance.PlayerProgressData.SoulCollected -= Cost;
        PlayerController.Instance.PlayerProgressData.SaveClaimReward();
        GachaTime++;

        // Instantiate the gacha item at the specified position
        Instantiate(gachaPrefab, OutputTransform.position, Quaternion.identity);
    }
}