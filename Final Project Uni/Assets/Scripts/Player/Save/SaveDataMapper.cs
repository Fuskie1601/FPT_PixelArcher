using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Required for .Select()
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

// Serializable Data Classes
[Serializable]
public class PlayerStatsData
{
    public int playerSoul;
    public int knowledgeLevel;
    public int permaHP_UpAmount;
    public int permaSpeed_UpAmount;
    public int permaDamage_UpAmount;
    public int permaStamina_UpAmount;
    public float bonusSpeed;
    public float bonusHealth;
}
[Serializable]
public class HealthData
{
    public string healthState;
    public int maxHealth;
    public int health;
    public int overHeal;
    public bool isTrapMaster;
}
[Serializable]
public class PlayerRunData
{
    public int KnowledgeLevel;
    public int Gold, GoldRecord;
    public int SoulCollected;
    public float SoulRecord;
    public List<SkillUnlock> unlockedSkills = new List<SkillUnlock>();
    public List<InventoryItem> Inventory = new List<InventoryItem>();
    public List<RecipeUnlock> unlockedRecipes = new List<RecipeUnlock>();
}

public static class DataCopier
{
    // Static method to copy data from PlayerStatsData to PlayerStats
    public static void CopyFromData(this PlayerStats target, PlayerStatsData source)
    {
        if (source == null || target == null) return;

        target.playerSoul = source.playerSoul;
        target.knowledgeLevel = source.knowledgeLevel;

        target.permaHP_UpAmount = source.permaHP_UpAmount;
        target.permaSpeed_UpAmount = source.permaSpeed_UpAmount;
        target.permaDamage_UpAmount = source.permaDamage_UpAmount;
        target.permaStamina_UpAmount = source.permaStamina_UpAmount;

        target.bonusSpeed = source.bonusSpeed;
        target.bonusHealth = source.bonusHealth;
    }

    // Static method to copy data from HealthData to Health
    public static void CopyFromData(this Health target, HealthData source)
    {
        if (source == null || target == null) return;

        target.healthState = Enum.TryParse(source.healthState, out HealthState state) ? state : HealthState.Idle;
        target.maxHealth = source.maxHealth;
        target.health = source.health;
        target.overHeal = source.overHeal;
        target.isTrapMaster = source.isTrapMaster;
    }

    // Static method to copy data from PlayerRunData to PlayerData
    public static void CopyFromData(this PlayerProgressData target, PlayerRunData source)
    {
        if (source == null || target == null) return;

        target.KnowledgeLevel = source.KnowledgeLevel;
        target.Gold = source.Gold;
        target.SoulCollected = source.SoulCollected;
        target.GoldRecord = source.GoldRecord;
        target.SoulRecord = source.SoulRecord;

        target.unlockedSkills = new List<SkillUnlock>(source.unlockedSkills);
        target.Inventory = new List<InventoryItem>(source.Inventory);
        target.unlockedRecipes = new List<RecipeUnlock>(source.unlockedRecipes);

        // Log Inventory items for debugging
        /*
        Debug.Log("Loaded Inventory!!");
        foreach (var item in target.Inventory)
        {
            //Debug.Log(item.item.itemName + " " + item.amount);
        }
        */
    }
}


public static class DataExtensions
{
    // PlayerStats
    public static void CopyFrom(this PlayerStats target, PlayerStats source)
    {
        if (source == null) return;

        // Copy primitive and value type fields
        target.playerSoul = source.playerSoul;
        target.knowledgeLevel = source.knowledgeLevel;

        target.permaHP_UpAmount = source.permaHP_UpAmount;
        target.permaSpeed_UpAmount = source.permaSpeed_UpAmount;
        target.permaDamage_UpAmount = source.permaDamage_UpAmount;
        target.permaStamina_UpAmount = source.permaStamina_UpAmount;

        target.bonusSpeed = source.bonusSpeed;
        target.bonusHealth = source.bonusHealth;

        // Add other necessary fields or properties here
    }


    public static void UpdateFrom(this PlayerStats target, PlayerStats source)
    {
        target.CopyFrom(source); // For simplicity, reuse the CopyFrom method
    }

    // Health
    public static void CopyFrom(this Health target, Health source)
    {
        if (source == null) return;

        target.healthState = source.healthState;
        target.maxHealth = source.maxHealth;

        // Directly copy the internal health field to avoid setter logic
        target.health = source.health; // Copy the value of 'health' directly
        target.overHeal = source.overHeal;

        target.isTrapMaster = source.isTrapMaster;
    }


    public static void UpdateFrom(this Health target, Health source)
    {
        target.CopyFrom(source);
    }

    // PlayerData
    public static void CopyFrom(this PlayerProgressData target, PlayerProgressData source)
    {
        if (source == null) return;
        target.KnowledgeLevel = source.KnowledgeLevel;
        target.Gold = source.Gold;
        target.SoulCollected = source.SoulCollected;
    
        // Copy skills, inventory, and recipes
        target.unlockedSkills = new List<SkillUnlock>(source.unlockedSkills);
        target.Inventory = new List<InventoryItem>(source.Inventory);
        target.unlockedRecipes = new List<RecipeUnlock>(source.unlockedRecipes);

        Debug.Log("Loaded Inventory!!");

        foreach (var item in target.Inventory)
        {
            Debug.Log(item.item.itemName + " " + item.amount);
        }
    }


    public static void UpdateFrom(this PlayerProgressData target, PlayerProgressData source)
    {
        target.CopyFrom(source);
    }
}


// Data Mapper Extension Methods
public static class SaveDataMapper
{
    // Map PlayerStats to PlayerStatsData
    public static PlayerStatsData ToData(this PlayerStats source)
    {
        return new PlayerStatsData
        {
            playerSoul = source.playerSoul,
            knowledgeLevel = source.knowledgeLevel,
            permaHP_UpAmount = source.permaHP_UpAmount,
            permaSpeed_UpAmount = source.permaSpeed_UpAmount,
            permaDamage_UpAmount = source.permaDamage_UpAmount,
            permaStamina_UpAmount = source.permaStamina_UpAmount,
            bonusSpeed = source.bonusSpeed,
            bonusHealth = source.bonusHealth
        };
    }

    // Map Health to HealthData
    public static HealthData ToData(this Health source)
    {
        return new HealthData
        {
            healthState = source.healthState.ToString(),
            maxHealth = source.maxHealth,
            health = source.health,
            overHeal = source.overHeal,
            isTrapMaster = source.isTrapMaster
        };
    }

    // Map PlayerData to PlayerRunData
    public static PlayerRunData ToData(this PlayerProgressData source)
    {
        return new PlayerRunData
        {
            KnowledgeLevel = source.KnowledgeLevel,
            Gold = source.Gold,
            SoulCollected = Mathf.RoundToInt(source.SoulCollected),
            unlockedSkills = new List<SkillUnlock>(source.unlockedSkills),
            Inventory = new List<InventoryItem>
            (
                source.Inventory.Select(item => new InventoryItem
                {
                    item = item.item,
                    amount = item.amount
                })
            ),
            unlockedRecipes = new List<RecipeUnlock>(source.unlockedRecipes)
        };
    }
    
    
    // Map PlayerStatsData to PlayerStats
    public static PlayerStats FromData(this PlayerStatsData source)
    {
        return new PlayerStats
        {
            playerSoul = source.playerSoul,
            knowledgeLevel = source.knowledgeLevel,
            permaHP_UpAmount = source.permaHP_UpAmount,
            permaSpeed_UpAmount = source.permaSpeed_UpAmount,
            permaDamage_UpAmount = source.permaDamage_UpAmount,
            permaStamina_UpAmount = source.permaStamina_UpAmount,
            bonusSpeed = source.bonusSpeed,
            bonusHealth = source.bonusHealth
        };
    }

    // Map HealthData to Health
    public static Health FromData(this HealthData source)
    {
        return new Health
        {
            healthState = Enum.TryParse(source.healthState, out HealthState state) ? state : HealthState.Idle, // Safe parsing
            maxHealth = source.maxHealth,
            health = source.health,
            overHeal = source.overHeal,
            isTrapMaster = source.isTrapMaster
        };
    }

    // Map PlayerRunData to PlayerData
    public static PlayerProgressData FromData(this PlayerRunData source)
    {
        var playerData = new PlayerProgressData
        {
            KnowledgeLevel = source.KnowledgeLevel,
            Gold = source.Gold,
            SoulCollected = source.SoulCollected,
            unlockedSkills = new List<SkillUnlock>(source.unlockedSkills),
            Inventory = new List<InventoryItem>(source.Inventory),
            unlockedRecipes = new List<RecipeUnlock>(source.unlockedRecipes)
        };

        return playerData;
    }
    
    
}


