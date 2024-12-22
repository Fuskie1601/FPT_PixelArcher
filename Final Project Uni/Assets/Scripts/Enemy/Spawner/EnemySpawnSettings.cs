using UnityEngine;
[CreateAssetMenu(fileName = "EnemySpawnSettings", menuName = "ScriptableObjects/EnemySpawnSettings")]
public class EnemySpawnSettings : ScriptableObject
{
    public string Biome;
    public int ID;
    public int easyCount;
    public int normalCount;
    public int hardCount;
    public int specialCount;
    public int bossesCount;
}