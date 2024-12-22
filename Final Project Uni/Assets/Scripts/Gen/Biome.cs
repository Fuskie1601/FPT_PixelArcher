using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public struct DirectionalLightSettings
{
    public Color lightColor;
    public float intensity;
    public Vector3 rotation;
}

[CreateAssetMenu(fileName = "Biome", menuName = "Gen/Biome")]
[System.Serializable]
public class Biome : ScriptableObject
{
    public String biomeName;
    public int GridSize = 100;
    public List<Room> GenericRoom;
    public List<Room> RewardRoom;
    public List<Room> PuzzleRoom;
    public List<Room> BossRoom;
    public Room startRoom;
    public Room exitRoom;
    
    [CanBeNull] public Material skybox;
    public DirectionalLightSettings lightSettings;
}


