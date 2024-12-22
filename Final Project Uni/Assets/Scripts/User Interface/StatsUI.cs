using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField, ReadOnly]
    PlayerStats playerStat;
    [SerializeField, ReadOnly]
    SkillHolder skillHolder;
    private List<GameObject> skillList = new List<GameObject>();
    [FoldoutGroup("Setup")]
    public List<Image> skillIconUI;
    [FoldoutGroup("Setup")]
    public List<Button> skillBtn;
    [FoldoutGroup("Setup")]
    public GameObject DescriptionObj;
    [FoldoutGroup("Setup")]
    public TMP_Text Name, Description, Type;
    [FoldoutGroup("Setup")]
    public Image DescriptionIcon;
    [FoldoutGroup("Player Stats")]
    
    private float baseHealth, bonusHealth, 
                  baseSpeed, bonusSpeed, 
                  baseStamina, bonusStamina, 
                  baseStaRegen, bonusStaRegen, 
                  baseAtk, bonusATK;
    private int level;
    [FoldoutGroup("Player Stats Texts")]
    public TMP_Text baseHealthText, bonusHealthText, 
                    baseSpeedText, bonusSpeedText, 
                    baseStaminaText, bonusStaminaText, 
                    baseStaRegenText, bonusStaRegenText, 
                    baseAtkText, bonusATKText,
                    levelText;

    private void OnEnable()
    {
        UpdateInfo();
    }

    // Start is called before the first frame update
    [Button]
    public void UpdateInfo()
    {
        playerStat = PlayerController.Instance._stats;
        skillHolder = SkillHolder.Instance;
        skillList = skillHolder.skillList;

        if (skillList.Count > 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                var skill = skillList[i].GetComponent<ISkill>();
                
                // Clear previous listeners to avoid duplication
                skillBtn[i].onClick.RemoveAllListeners();

                if (skill != null)
                {
                    skillIconUI[i].sprite = skill.Icon;
                    skillBtn[i].onClick.AddListener(() => SetSkillDes(skill));
                }
            }
        }
        
        ShowStat();
    }

    public void SetSkillDes(ISkill skill)
    {
        DescriptionIcon.sprite = skill.Icon;
        
        Debug.Log(skill.Name);
        
        string formattedName = skill.Name.Replace("_", " ");
        Name.text = formattedName;
        
        Description.text = skill.Description;
        Type.text = skill.type.ToString();
        DescriptionObj.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void ShowStat()
    {
        baseHealth = (playerStat.totalMaxHealth - playerStat.bonusHealth);
        bonusHealth = (playerStat.bonusHealth);
        baseSpeed = playerStat.speed - playerStat.bonusSpeed;
        bonusSpeed = playerStat.bonusSpeed;
        baseStamina = (int)(playerStat.maxStamina - playerStat.bonusMaxStamina);
        bonusStamina = (playerStat.bonusMaxStamina);
        baseStaRegen = (playerStat.regenRate - playerStat.bonusRegenRate);
        bonusStaRegen = (playerStat.bonusRegenRate);
        baseAtk = (playerStat.defaultDamage * playerStat.PermaDamage_Percent);
        bonusATK = (playerStat.bonusDamage);
        level = (playerStat.knowledgeLevel);

        baseHealthText.text = "" + baseHealth.ToString("0.0"); 
        bonusHealthText.text = bonusHealth > 0 ? "+" + bonusHealth.ToString("0.0") : ""; 
        baseSpeedText.text = "" + baseSpeed.ToString("0.0"); 
        bonusSpeedText.text = bonusSpeed > 0 ? "+" + bonusSpeed.ToString("0.0") : "";
        baseStaminaText.text = "" + baseStamina;
        bonusStaminaText.text = bonusStamina > 0 ? "+" + bonusStamina.ToString("0.0") : ""; 
        baseStaRegenText.text = "" + baseStaRegen.ToString("0.0"); 
        bonusStaRegenText.text = bonusStaRegen > 0 ? "+" + bonusStaRegen.ToString("0.0") : ""; 
        baseAtkText.text = "" + baseAtk.ToString("0.0"); 
        bonusATKText.text = bonusATK > 0 ? "+" + bonusATK.ToString("0.0") : "";
        levelText.text = level.ToString();
    }

}
