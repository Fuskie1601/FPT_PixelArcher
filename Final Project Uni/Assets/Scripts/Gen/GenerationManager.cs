using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private RoomGen Gen;
    [SerializeField] private int GridSize = 1;
    [SerializeField] private int MainPathLength = 4;
    [SerializeField] private int SideRoomChance;
    [SerializeField] private Biome biome;
    [SerializeField] private String BiomeName;
    [SerializeField] private int MinPuzzleRoom;
    [SerializeField] private int MaxPuzzleRoom;
    [SerializeField] private int MinRewardRoom;
    [SerializeField] private int MaxRewardRoom;
    private List<Room> GenericRoom;
    private List<Room> RewardRoom;
    private List<Room> PuzzleRoom;
    private List<Room> BossRoom;
    private bool haveBoss;
    private Room StartRoom;
    private Room ExitRoom;

    public void Start()
    {
        player = PlayerController.Instance.PlayerRB.gameObject;

    }
    public async void Generate()
    {
        // Clear the generator and assign data
        
        player.transform.position = new Vector3(0,-500,0);
        Gen.Clear();
        AssignDataToRoomGen();

        // Start the generation process
        Gen.Generate();

        // Check if the player is null and assign it when PlayerController.Instance and PlayerRB are available
        await UniTask.WaitUntil(() => PlayerController.Instance != null && PlayerController.Instance.PlayerRB != null);

        if (player == null) player = PlayerController.Instance.PlayerRB.gameObject;
        
        // Set the player's position
        player.transform.position = Gen.spawn;
    }

    public void Clear()
    {
        Gen.Clear();
    }


    public void AssignDataToRoomGen()
    {
        Gen.AssignData(GridSize, MainPathLength, UnityEngine.Random.Range(MinPuzzleRoom, MaxPuzzleRoom), UnityEngine.Random.Range(MinRewardRoom, MaxRewardRoom), GenericRoom, RewardRoom, PuzzleRoom, BossRoom, haveBoss, StartRoom, ExitRoom);
    }
    public void LoadBiomeData(Biome biome)
    {
        this.biome = biome;
        BiomeName = biome.biomeName;
        GridSize = biome.GridSize;
        GenericRoom = biome.GenericRoom;
        RewardRoom = biome.RewardRoom;
        PuzzleRoom = biome.PuzzleRoom;
        BossRoom = biome.BossRoom;
        StartRoom = biome.startRoom;
        ExitRoom = biome.exitRoom;
        
        if(biome.skybox != null)
        RenderSettings.skybox = biome.skybox;

        ApplyDirectionalLightSettings(biome.lightSettings);
    }
    public void LoadFloorData(Floor newFloor)
    {

        MainPathLength = newFloor.MainPathLength;
        SideRoomChance = newFloor.SideRoomChance;
        MinPuzzleRoom = newFloor.MinPuzzleRoom;
        MaxPuzzleRoom = newFloor.MaxPuzzleRoom;
        MinRewardRoom = newFloor.MinRewardRoom;
        MaxRewardRoom = newFloor.MaxRewardRoom;
        haveBoss = newFloor.haveBoss;
        //StartRoom = newFloor.StartRoom;
        //EndRoom = newFloor.EndRoom;
    }
    public void LoadNewBiome()
    {
        BiomeName = biome.biomeName;
        GenericRoom = biome.GenericRoom;
        RewardRoom = biome.RewardRoom;
        PuzzleRoom = biome.PuzzleRoom;
        StartRoom = biome.startRoom;
        ExitRoom = biome.exitRoom;
    }

    public void ApplyDirectionalLightSettings(DirectionalLightSettings lightSettings)
    {
        // Find the Directional Light in the scene
        Light directionalLight = FindDirectionalLight();

        if (directionalLight == null)
        {
            Debug.LogWarning("No Directional Light found in the scene.");
            return;
        }

        // Apply the settings
        directionalLight.color = lightSettings.lightColor;
        directionalLight.intensity = lightSettings.intensity;
        directionalLight.transform.rotation = Quaternion.Euler(lightSettings.rotation);

        //Debug.Log($"Applied directional light settings from biome '{biome.biomeName}'.");
    }
    
    private Light FindDirectionalLight()
    {
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        foreach (var light in lights)
        {
            if (light.type == LightType.Directional)
            {
                return light;
            }
        }

        return null;
    }
}
