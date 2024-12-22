using System;
using System.Collections;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerSoul = 0;
    public int knowledgeLevel;

    #region Default
    [SerializeField]
    private int defaultMaxHealth = 10;

    [FoldoutGroup("Default Stats")]
    public float defaultSpeed = 0.7f, defaultRotationSpeed = 25, defaultMaxSpeed = 20;

    [FoldoutGroup("Default Stats/Roll")]
    public float defaultRollSpeed = 13, defaultRollCD = 0.6f, defaultRollTime = 0.25f, defaultControlRollDirect = 0.2f;

    [FoldoutGroup("Default Stats/Shooting")]
    public float defaultChargedTime = 2;

    [FoldoutGroup("Default Stats/Guard")]
    public float defaultGuardTime = 2; //temp

    [FoldoutGroup("Default Stats/Stamina")]
    public int defaultMaxStamina = 100, defaultRegenRate = 10, defaultStaminaRollCost = 20;

    [FoldoutGroup("Default Stats/Arrow")]
    public float defaultRicochetFriction = 0, defaultRecallSpeed = 40;

    [FoldoutGroup("Default Stats/Arrow")]
    public int defaultDamage = 2;

    [FoldoutGroup("Default Stats/Arrow")]
    public const float defaultDamageMultiplier = 1;

    [FoldoutGroup("Default Stats/Physics")]
    [ReadOnly]
    public float defaultDrag, defaultMass;

    #endregion

    #region Perma Upgrade
    // Số lần đã upgrade
    [FoldoutGroup("Permanent Stats")]
    public int permaHP_UpAmount = 0, permaSpeed_UpAmount = 0, permaDamage_UpAmount = 0;
    [FoldoutGroup("Permanent Stats")]
    public int permaStamina_UpAmount = 0;
    
    [FoldoutGroup("Permanent Stats/Debug Calculate")]
    [ReadOnly] public float PermaHP_Percent = 1f, PermaSpeed_Percent = 1f, PermaDamage_Percent = 1f;
    [FoldoutGroup("Permanent Stats/Debug Calculate")]
    [ReadOnly] public float PermaStamina_Percent = 1f, PermaStaminaRegen_Percent = 1f;
    #endregion

    #region Bonus

    [FoldoutGroup("Bonus Stats")]
    public float bonusSpeed, bonusHealth, bonusRotationSpeed, bonusMaxSpeed;

    [FoldoutGroup("Bonus Stats/Roll")]
    public float bonusRollCD, bonusRollTime, bonusControlRollDirect;

    [FoldoutGroup("Bonus Stats/Stamina")]
    public int bonusMaxStamina, bonusRegenRate, bonusStaminaRollCost;

    [FoldoutGroup("Bonus Stats/Arrow Controller")]
    public float bonusChargedTime;

    [FoldoutGroup("Bonus Stats/Arrow")]
    public float bonusRicochetMultiplier;

    [FoldoutGroup("Bonus Stats/Arrow")] 
    public int bonusDamage; 
    [FoldoutGroup("Bonus Stats/Arrow")] 
    public float bonusRecallSpeed;

    [FoldoutGroup("Bonus Stats/Arrow")]
    public float bonusDamageMultiplier;

    #endregion

    #region Total Value (Calculate)

    //Health
    public int totalMaxHealth => Mathf.CeilToInt((defaultMaxHealth * PermaHP_Percent) + bonusHealth);

    //Speed
    public float speed => (defaultSpeed * PermaSpeed_Percent) + bonusSpeed;
    public float rotationSpeed => defaultRotationSpeed + bonusRotationSpeed;
    public float maxSpeed => defaultMaxSpeed + bonusMaxSpeed;

    //Roll
    public float rollSpeed => defaultRollSpeed + (PermaSpeed_Percent * defaultSpeed) + bonusSpeed;
    public float rollCD => defaultRollCD - bonusRollCD;
    public float rollTime => defaultRollTime + bonusRollTime;
    public float controlRollDirect => defaultControlRollDirect + bonusControlRollDirect;

    //Guard
    public float guardTime => defaultGuardTime; // No bonus, just returns default value

    //Stamina
    public int maxStamina => Mathf.CeilToInt(defaultMaxStamina * PermaStamina_Percent) + bonusMaxStamina;
    public int regenRate => Mathf.CeilToInt(defaultRegenRate * PermaStaminaRegen_Percent) + bonusRegenRate;
    public int staminaRollCost => defaultStaminaRollCost - bonusStaminaRollCost;

    //Arrow Controller
    public float ChargedTime => defaultChargedTime - bonusChargedTime;

    //Arrow
    public float staticFriction => defaultRicochetFriction * bonusRicochetMultiplier;
    public float recallSpeed => defaultRecallSpeed + bonusRecallSpeed;

    public int Damage => Mathf.CeilToInt(((defaultDamage * PermaDamage_Percent) + bonusDamage) * DamageMultiplier);
    public float DamageMultiplier => defaultDamageMultiplier + bonusDamageMultiplier;

    #endregion

    [SerializeField, ReadOnly] private PlayerController _pc;
    private ArrowController _arrowController;
    
    private void Start()
    {
        _pc = PlayerController.Instance;
        _arrowController = _pc._arrowController;
        defaultDrag = _pc.PlayerRB.drag;
        defaultMass = _pc.PlayerRB.mass;
        LoadSave();
    }
    
    //in-game buff
    public void BuffPlayer(BuffType type, float amount = 0)
    {
        switch (type)
        {
            case BuffType.Health:
                bonusHealth += amount;
                
                //Set HP
                _pc.PlayerHealth.maxHealth = totalMaxHealth;
                _pc.PlayerHealth.currentHealth = totalMaxHealth;
                break;
            case BuffType.Damage:
                bonusDamage += Mathf.CeilToInt(amount);
                break;
            case BuffType.Speed:
                bonusSpeed += amount;
                break;
        }

        UpdateStats();
    }

    public void SetBuffATKMul(float amount)
    {
        bonusDamageMultiplier = Mathf.CeilToInt(amount);
        UpdateStats();
    }

    [Button]
    public void UpdateStats()
    {
        //Stamina
        _pc.staminaSystem.MaxStamina = maxStamina;
        _pc.staminaSystem.RegenRate = regenRate;

        //Arrow Controller
        _arrowController.chargedTime = ChargedTime;

        //Arrow
        foreach (var arrow in _arrowController.arrowsList)
        {
            arrow.bonusRicochetMultiplier = bonusRicochetMultiplier;
            arrow.hitbox.BaseDamage = Damage;
            arrow.recallSpeed = recallSpeed;
        }

        //health
        _pc.PlayerHealth.maxHealth = totalMaxHealth;
    }

    public void LoadSave()
    {
        PermaStatsData loadedData = PlayerDataCRUD.LoadPermanentStats();
        playerSoul = loadedData.Soul;
        knowledgeLevel = loadedData.knowledgeLevel;
        permaHP_UpAmount = loadedData.HPUpgradesData;
        permaSpeed_UpAmount = loadedData.SpeedUpgradesData;
        permaDamage_UpAmount = loadedData.DamageUpgradesData;
        permaStamina_UpAmount = loadedData.StaminaUpgradesData;
        
        if(AchievementManager.instance != null)
        AchievementManager.instance.SetAchievementProgress("Smartie", knowledgeLevel);

        UpdatePermaPercent();
        UpdateStats();
    }

    public void UpdatePermaPercent()
    {
        PermaHP_Percent = 1f + (CalculateMultiplier(permaHP_UpAmount));
        PermaSpeed_Percent = 1f + (CalculateMultiplier(permaSpeed_UpAmount));
        PermaDamage_Percent = 1f + (CalculateMultiplier(permaDamage_UpAmount));
        PermaStamina_Percent = 1f + (CalculateMultiplier(permaStamina_UpAmount));
    }
    
    [Button]
    public void ResetBonus()
    {
        // Reset bonus stats
        bonusSpeed = 0;
        bonusHealth = 0;
        bonusRotationSpeed = 0;
        bonusMaxSpeed = 0;

        // Roll-related bonuses
        bonusRollCD = 0;
        bonusRollTime = 0;
        bonusControlRollDirect = 0;

        // Stamina-related bonuses
        bonusMaxStamina = 0;
        bonusRegenRate = 0;
        bonusStaminaRollCost = 0;

        // Arrow Controller bonuses
        bonusChargedTime = 0;

        // Arrow bonuses
        bonusRicochetMultiplier = 0;
        bonusDamage = 0;
        bonusRecallSpeed = 0;
        bonusDamageMultiplier = 0;

        // Update stats to apply the reset
        UpdateStats();
    }

    
    public float CalculateMultiplier(int amount)
    {
        return (Mathf.Pow(amount, 0.6f)) * 0.01f;
    }
}

