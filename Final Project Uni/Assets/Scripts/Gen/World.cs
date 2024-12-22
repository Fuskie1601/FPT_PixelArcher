using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Gen/World")]
[System.Serializable]

public class World : ScriptableObject
{
    public string worldName;
    public int SideRoomChance = 5;
    public int MainPathLength = 5;
    public int difficulty = 1;
    public int MinPuzzleRoom;
    public int MaxPuzzleRoom;
    public int MinRewardRoom;
    public int MaxRewardRoom;

    public List<Biome> biomePool;
    public List<Floor> floors;
}
