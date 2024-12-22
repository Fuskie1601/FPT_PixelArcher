using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public enum SkillType
{
    PASSIVE, ACTIVE
}
public abstract class ISkill : MonoBehaviour
{
    [FoldoutGroup("Base Skill")] 
    [CanBeNull] public Sprite Icon;
    [FoldoutGroup("Base Skill")]
    [SerializeField] public PlayerController _pc;
    [FoldoutGroup("Base Skill")]
    public string Name;
    [FoldoutGroup("Base Skill")]
    public float Cooldown, currentCD;
    [FoldoutGroup("Base Skill")]
    public SkillType type;

    [TextArea]
    public string Description;
    public virtual void Activate() { }
    public virtual void Deactivate() { }
    void Update()
    {
        CooldownTimer();
    }
    public async UniTaskVoid Assign()
    {
        _pc = PlayerController.Instance;
        _pc.PlayerProgressData.UnlockSkill(Name);
    }
    #region Timer
    public void CooldownTimer()
    {
        if (currentCD >= 0)
            currentCD -= Time.deltaTime;
    }

    #endregion
}
