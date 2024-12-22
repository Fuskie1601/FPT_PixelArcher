    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "SkillDatabase", menuName = "ScriptableObjects/SkillDatabase", order = 1)]
    public class SkillDatabase : ScriptableObject
    {
        public List<PlayerSkill> allSkills; // List of all skills in the game
    }