using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ExpeditionManager : MonoBehaviour
{
    public GenerationManager gen;
    [FoldoutGroup("Debug")]
    public List<World> worlds;
    [FoldoutGroup("Debug")]
    public List<Floor> floors;
    [FoldoutGroup("Debug")]
    public World currentWorld;
    [FoldoutGroup("Debug")]
    public Biome currentBiome;
    [FoldoutGroup("Debug")]
    public Floor currFloor;
    [FoldoutGroup("Expedition Number")]
    public int currentWorldNumber = 0;
    [FoldoutGroup("Expedition Number")]
    public int currentFloorNumber = 0;
    [FoldoutGroup("Event")]
    public UnityEvent OnFloorExit, OnWorldExit, LoadNextFloor, SkillEvent, OnTransition, OnExpeditionStart, OnExpeditionExit;

    public static ExpeditionManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadSave()
    {
        if (Ingame_Save.Instance.haveFileLoad)
        {
            Debug.Log("Load " + Ingame_Save.Instance);
            currentWorldNumber = Ingame_Save.Instance.World;
            currentFloorNumber = Ingame_Save.Instance.Floor;
            currentBiome = Ingame_Save.Instance.currentBiome;
            currentWorld = Ingame_Save.Instance.currentWorld;
        }
    }

    public void NewRun()
    {
        Debug.Log("new run");
        ExpeditionStart(0, 0);
    }

    public async UniTask LoadProgress()
    {
        Debug.Log("load then run");
        try
        {
            await UniTask.WaitUntil(() => Ingame_Save.Instance != null);
            LoadSave();
            ExpeditionStart(currentWorldNumber, currentFloorNumber);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in WaitForSaveAndStartExpedition: " + ex.Message);
        }
    }

    #region Event

    //Load next floor after event 
    //[Button]
    public void NextFloor()
    {
        Debug.Log("World : " + currentWorldNumber + " - " + " Floor :" + currentFloorNumber);
        GenerateFloor(currentFloorNumber);
        LoadNextFloor.Invoke();
    }

    //setup number pre-event to prepare for next floor load
    [Button]
    public void ExitFloor()
    {
        LoadFloor();
    }

    async void LoadFloor()
    {
        Debug.Log("try to Exit");
        GameManager.Instance.fadeInAnim.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        doExitFloor();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        GameManager.Instance.fadeOutAnim.Invoke();
    }


    void doExitFloor()
    {
        PlayerController.Instance.PlayerProgressData.SaveClaimReward();
        //Ingame_Save.Instance.Save(); //bring to Exit Event
        if (currentFloorNumber + 1 < floors.Count)
        {
            CheckEvent();
            currentFloorNumber += 1;
            OnFloorExit.Invoke();
        }
        else if (currentWorldNumber + 1 < worlds.Count)
        {
            CheckEvent();
            currentWorldNumber += 1;
            currentFloorNumber = 0;
            SetWorld(currentWorldNumber);
            OnWorldExit.Invoke();
        }
        else
        {
            ExpeditionComplete();
        }
    }
    public void CheckEvent()
    {
        Floor f = floors[currentFloorNumber];
        ExpeditionEvent ex = f.exEvent;
        if (ex.eventType == EventType.SkillChoose)
            SkillEvent.Invoke();
        else
            OnTransition.Invoke();
    }
    public bool CheckBossRoom()
    {
        return floors[currentFloorNumber].haveBoss;
    }
    public void ExpeditionStart(int world, int floor)
    {
        SetWorld(world);
        GenerateFloor(floor);
        OnExpeditionStart.Invoke();
    }
    public void ExpeditionComplete()
    {
        currentWorldNumber = 0;
        currentFloorNumber = 0;
        OnExpeditionExit.Invoke();
    }

    #endregion

    #region GenerationManager
    public void GenerateFloor(int floor)
    {
        SetFloorData(floor);
        gen.Generate();
    }
    public void Gen()
    {
        GenerateFloor(currentFloorNumber);
    }

    void SetWorld(int worldIndex)
    {
        currentWorld = worlds[worldIndex];
        currentWorldNumber = worldIndex;
        ChooseRandomBiome();
        floors = currentWorld.floors;
    }
    void ChooseRandomBiome()
    {
        List<Biome> biomes = currentWorld.biomePool;
        currentBiome = biomes[UnityEngine.Random.Range(0, biomes.Count)];
        gen.LoadBiomeData(currentBiome);
    }

    [Button]
    public void SetFloorData(int floor)
    {
        //Debug.Log("Set Floor " + (floor + 1));
        currFloor = floors[floor];
        gen.LoadFloorData(currFloor);
    }


    public void PlayBGMBiome()
    {
        Debug.Log("BGM " + currentBiome.biomeName);
        AudioManager.Instance.ChangeMusic(currentBiome.biomeName);
    }
    #endregion

}
