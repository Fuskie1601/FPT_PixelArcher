using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerSkill
{
    public string Skill_ID;
    public GameObject skillPrefab;
    public bool defaultUnlocked;
}

[System.Serializable]
public class SkillUnlock
{
    public PlayerSkill Skill;
    public bool isUnlocked;
}