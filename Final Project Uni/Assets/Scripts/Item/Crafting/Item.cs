using System;
using JetBrains.Annotations;
using PA;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum BuffType
{
    Health, Speed, Damage
}

public class Item : MonoBehaviour
{
    [FoldoutGroup("Base")]
    [CanBeNull] public Image itemSprite;
    
    [FoldoutGroup("Buff")]
    public BuffType type;
    [FoldoutGroup("Buff")]
    public float amount;
    
    [FoldoutGroup("Skill")]
    [CanBeNull] public bool takeSkillIcon = true;
    [FoldoutGroup("Skill")]
    [CanBeNull] public GameObject Skill;

    [FoldoutGroup("Material")] 
    public string itemID;
    [FoldoutGroup("Material")] 
    public int itemAmount;
    
    [FoldoutGroup("Currency")]
    public float MinGold, MaxGold;
    [FoldoutGroup("Currency")]
    public float MinSoul, MaxSoul;

    [FoldoutGroup("Debug")]
    [ReadOnly] public bool isSkillBuff;
    
    public void Awake()
    {
        isSkillBuff = (Skill != null);
        
        if (Skill != null && itemSprite != null && 
            takeSkillIcon && Skill.TryGetComponent<ISkill>(out ISkill skill))
        {
            itemSprite.sprite = skill.Icon;
        }
    }

    public void GrabItem()
    {
        ExpeditionReport.Instance.ItemCollected += 1;
        
        GrabMaterial();
        ApplyBuff();
        GrabSkill();
        GrabMoney();
    }
    
    public void GrabSkill()
    {
        if(Skill != null) SkillHolder.Instance.AddSkill(Skill);
    }
    public void ApplyBuff()
    {
        if(amount != 0) PlayerController.Instance._stats.BuffPlayer(type, amount);
    }
    public void GrabMoney()
    {
        // Generate random Gold and Soul values within the specified ranges
        float goldAmount = UnityEngine.Random.Range(MinGold, MaxGold);
        float soulAmount = UnityEngine.Random.Range(MinSoul, MaxSoul);

        if (PlayerController.Instance.PlayerProgressData == null)
            PlayerController.Instance.PlayerProgressData = Player_Singleton.Instance.GetComponentInChildren<PlayerProgressData>();
        if (goldAmount > 0 || soulAmount > 0)
        {
            PlayerController.Instance.PlayerProgressData.AddCurrency(goldAmount, soulAmount);
        }
    }

    public void GrabMaterial()
    {
        PlayerController.Instance.PlayerProgressData.AddItem(itemID, itemAmount);
    }
 }
