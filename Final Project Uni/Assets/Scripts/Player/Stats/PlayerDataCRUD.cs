using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PlayerDataCRUD
{
    private static readonly string PlayerDataSavePath = Path.Combine(Application.persistentDataPath, "player_data.json");
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "player_perma_stats.json");
    
    
    public static void SavePlayerData(PlayerDataSave data)
    {
        // Serialize the PlayerDataSave object to JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PlayerDataSavePath, json);
        Debug.Log($"Player data saved at {PlayerDataSavePath}");
    }
    public static PlayerDataSave LoadPlayerData()
    {
        if (File.Exists(PlayerDataSavePath))
        {
            string json = File.ReadAllText(PlayerDataSavePath);
            return JsonUtility.FromJson<PlayerDataSave>(json);
        }
        return null;
    }
    
    public static void SavePermanentStats(PermaStatsData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Perma stats saved at {SavePath}");
    }
    public static PermaStatsData LoadPermanentStats()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<PermaStatsData>(json);
        }
        return new PermaStatsData();
    }
}