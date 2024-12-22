using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ReportElement
{
    public Sprite icon;
    public string text;
}

public class ExpeditionReport : MonoBehaviour
{
    public int EnemyDefeated = 0, BossDefeated = 0;
    public int ChestOpened = 0, ItemCollected = 0;
    public int RoomDiscovered = 0;
    public int KnowledgeLevelCollected = 0, GoldCollected = 0, SoulCollected = 0;
    public int DamageDeal = 0, SignBreak = 0;

    public int DieTimes = 0;
    //public List<string> Notif;
    public List<ReportElement> ReportElements;

    public static ExpeditionReport Instance;
    
    public void Start()
    {
        if (Instance == null) Instance = this;
    }

    public void AddReportElement(string itemName, Sprite icon)
    {
        ReportElement reportElement = new ReportElement();
        if(icon != null) reportElement.icon = icon;
        reportElement.text = itemName;
        ExpeditionReport.Instance.ReportElements.Add(reportElement);
    }

    public void AchievementProgress()
    {
        AchievementManager.instance.AddAchievementProgress("Monster Hunter", EnemyDefeated);
        AchievementManager.instance.AddAchievementProgress("Monster Slayer", EnemyDefeated);
        AchievementManager.instance.AddAchievementProgress("Monster Nightmare", EnemyDefeated);
        AchievementManager.instance.AddAchievementProgress("Overlord", BossDefeated);
        AchievementManager.instance.AddAchievementProgress("Chest Opener", ChestOpened);
        AchievementManager.instance.AddAchievementProgress("Trailblazer", RoomDiscovered);
        AchievementManager.instance.AddAchievementProgress("Board Cutter", SignBreak);
        if(SignBreak > 0) AchievementManager.instance.Unlock("Half of the Truth is not the Truth");
        AchievementManager.instance.AddAchievementProgress("No Pain No Gain", DieTimes);
    }
}
