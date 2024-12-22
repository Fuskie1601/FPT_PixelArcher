using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EnemyDifficulty
{
    Easy, Normal, Hard, Special, Bosses
}
[Serializable]
public struct EnemySpawn
{
    public EnemyDifficulty enemyDifficulty;
    public GameObject EnemyPrefab;
}

[CreateAssetMenu(fileName = "EnemyBiomePool", menuName = "ScriptableObjects/EnemyBiomePool")]
public class EnemyBiomePool : ScriptableObject
{
    public string Biome;
    public int ID;
    
    public List<EnemySpawn> enemyPool;
}