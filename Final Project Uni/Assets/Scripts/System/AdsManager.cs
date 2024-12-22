using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    public UnityEvent Reward;

    
    void Awake()
    {
#if UNITY_IOS || UNITY_ANDROID
        if (Instance == null)
        {
            Instance = this;
            Gley.MobileAds.API.Initialize();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
#else
        Debug.LogWarning("AdsManager is disabled on platforms other than iOS or Android.");
        Destroy(gameObject); // Remove this component on unsupported platforms
#endif
    }
    
    public void ShowRewardedVideo()
    {
        if (Gley.MobileAds.API.IsRewardedVideoAvailable())
        Gley.MobileAds.API.ShowRewardedVideo(CompleteMethod);
    }
    
    public void ShowInterstitial()
    {
        if (Gley.MobileAds.API.IsInterstitialAvailable())
        Gley.MobileAds.API.ShowInterstitial();
    }
    
    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            Reward.Invoke();
            Reward.RemoveAllListeners();
        }
    }
}
