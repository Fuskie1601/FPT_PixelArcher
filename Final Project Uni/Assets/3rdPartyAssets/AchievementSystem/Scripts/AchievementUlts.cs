using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct AchievementEvent
{
    public string Key;
    public UnityEvent Event;
}

public class AchievementUlts : MonoBehaviour
{
    [SerializeField, CanBeNull] private GameObject viewObj;
    public List<AchievementEvent> AchievementEventList;

    private void Start()
    {
        if(viewObj != null)
        viewObj.SetActive(false);
    }

    public void TogglePlayerUI(bool toggle) //convinient XD
    {
        ToggleUIElements.Instance.ToggleUI(toggle);
    }
    
    public void UnlockSkill(string SkillID)
    {
        PlayerController.Instance.PlayerProgressData.UnlockSkill(SkillID);
    }
    
    public void UnlockRecipe(string RecipeID)
    {
        PlayerController.Instance.PlayerProgressData.UnlockRecipe(RecipeID);
    }
    
    public void Add1(string Name)
    {
        AchievementManager.instance.AddAchievementProgress(Name, 1);
    }

    public void AwardSoul(int soulAmount)
    {
        // Load existing Soul value
        PermaStatsData permaStats = PlayerDataCRUD.LoadPermanentStats();
        permaStats.Soul += Mathf.RoundToInt(soulAmount);
        
        PlayerDataCRUD.SavePermanentStats(permaStats);
    }
    
    //Event Test
    [Button]
    public void Add(string Name, float amount)
    {
        AchievementManager.instance.AddAchievementProgress(Name, amount);
    }
    
    [Button]
    public void Unlock(string Name)
    {
        AchievementManager.instance.Unlock(Name);
    }
}
