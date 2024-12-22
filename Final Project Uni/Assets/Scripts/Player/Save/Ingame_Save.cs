using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Ingame_Save : MonoBehaviour
{
    public bool AutoRemoveProgressSave = true;
    // Fields changed to Data classes
    public PlayerStatsData Stats;
    public HealthData playerHealth;
    public PlayerRunData runData;

    public bool haveProgressSave_Debug;
    public List<string> skillList = new List<string>();

    [FoldoutGroup("Expedition")]
    public int Floor, World;
    [FoldoutGroup("Expedition")]
    public World currentWorld;
    [FoldoutGroup("Expedition")]
    public Biome currentBiome;

    private string saveFilePath;

    public static Ingame_Save Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        saveFilePath = Path.Combine(Application.persistentDataPath, "ExpeditionSaveData.json");
        haveProgressSave_Debug = File.Exists(saveFilePath); // Check if save file exists at startup
    }

    private void Start()
    {
        Load();  // Load data at the start
    }

    [Button]
    public void Save()
    {
        // Convert Player objects to Data objects using ToData()
        var clonedStats = PlayerController.Instance._stats.ToData();
        var clonedHealth = PlayerController.Instance.PlayerHealth.ToData();
        var clonedRunData = PlayerController.Instance.PlayerProgressData.ToData();

        // Assign the converted data to Ingame_Save fields
        Stats = clonedStats;  
        playerHealth = clonedHealth; 
        runData = clonedRunData;

        // Collect additional game data
        skillList = SkillHolder.Instance.SkillIDList;
        currentWorld = ExpeditionManager.Instance.currentWorld;
        currentBiome = ExpeditionManager.Instance.currentBiome;
        Floor = ExpeditionManager.Instance.currentFloorNumber;
        World = ExpeditionManager.Instance.currentWorldNumber;

        // Serialize to JSON
        string jsonData = JsonUtility.ToJson(this, true);
        File.WriteAllText(saveFilePath, jsonData);
        //haveProgressSave_Debug = true;

        Debug.Log("Game saved successfully!");
    }

    [Button]
    public async void Load()
    {
        // Read JSON file
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            JsonUtility.FromJsonOverwrite(jsonData, this);

            // Check if the current scene is "TestGenMap"
            if (SceneManager.GetActiveScene().name != "TestGenMap")
            {
                Debug.Log("Skipping skill loading in 'UI Main Menu' scene.");
                return;
            }

            // Apply the loaded data to the PlayerController components
            PlayerController.Instance.PlayerHealth.CopyFromData(playerHealth);
            PlayerController.Instance._stats.CopyFromData(Stats);
            PlayerController.Instance.PlayerProgressData.CopyFromData(runData);
            
            PlayerController.Instance.UpdateUI(runData.Gold.ToString(), 
                (runData.SoulCollected + PlayerController.Instance._stats.playerSoul).ToString());

            

            // Load skills
            foreach (var skill in SkillHolder.Instance.skillList)
            {
                if (skill != null) Destroy(skill); // Destroy the skill object
            }
            SkillHolder.Instance.skillList.Clear();
            SkillHolder.Instance.SkillIDList.Clear();
            SkillHolder.Instance.SkillOBJNameList.Clear();
            foreach (var skillID in skillList)
            {
                SkillHolder.Instance.AddSkillWithID(skillID);
            }

            await WaitUntilExpeditionManagerInitialized();

            // Expedition Load
            ExpeditionManager.Instance.currentWorld = currentWorld;
            ExpeditionManager.Instance.currentBiome = currentBiome;
            
            ExpeditionManager.Instance.PlayBGMBiome();
            
            //Delete the save file after loading it so it can't be used again
            //if (AutoRemoveProgressSave)
            DestroySave();

            Debug.Log("Game Progress loaded successfully!");
        }
        else
        {
            //ExpeditionManager.Instance.PlayBGMBiome();
            Debug.Log("Save file not found!");
        }
    }
    
    private async Task WaitUntilExpeditionManagerInitialized()
    {
        while (ExpeditionManager.Instance == null)
        {
            await Task.Yield(); // Wait until the next frame
        }
    }

    public void DestroySave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            //haveProgressSave_Debug = false;
            Debug.Log("Save data deleted successfully!");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

    
    public bool haveFileLoad
    {
        get
        {
            return File.Exists(saveFilePath) && Path.GetFileName(saveFilePath) == "ExpeditionSaveData.json";
        }
    }
}
