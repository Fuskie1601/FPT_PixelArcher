using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class BotMain : MonoBehaviour
{
    //Data
    public UnitType unitType;
    //public int Team = 0;
    
    [FoldoutGroup("Movement Settings")]
    public float minRange, maxRange, MoveAngle = 80f;
    [FoldoutGroup("Movement Settings")]
    public float cooldown = 2f, countdown;
    [FoldoutGroup("Movement Settings")]
    public float attackRange = 1.5f;
    [FoldoutGroup("Movement Settings/Prediction")]
    public bool UseMovementPrediction;
    [FoldoutGroup("Movement Settings/Prediction")]
    [Range(-1, 1)] public float MovementPredictionThreshold = 0;
    [FoldoutGroup("Movement Settings/Prediction")]
    [Range(0.25f, 2f)] public float MovementPredictionTime = 1f;

    [FoldoutGroup("Setup")]
    public Rigidbody rg;
    [FoldoutGroup("Setup")]
    public Health Health;
    [FoldoutGroup("Setup")]
    public List<BotGun> gun;
    [FoldoutGroup("Setup")]
    [CanBeNull] public BotAnimController _animController;
    [FoldoutGroup("Setup")]
    [CanBeNull] public UnityEvent ShootEvent;

    public void ResetCooldown()
    {
        countdown = cooldown;
    }
    public void Dead()
    {
        if (unitType == UnitType.BossPattern || unitType == UnitType.BossShooter) 
            ExpeditionReport.Instance.BossDefeated += 1;
        
        //Debug.Log("yes");
        if(ExpeditionReport.Instance != null) 
        ExpeditionReport.Instance.EnemyDefeated += 1;
        
        Destroy(this.gameObject);
    }

    public void Shoot(int gunSlot)
    {
        if(gun.Count <= 0) return;
        if(gun[gunSlot] != null && gun[gunSlot].target != null) gun[gunSlot].Fire();
        ShootEvent.Invoke();
    }

    public void playSoundEffect(string soundEffect)
    {
        AudioManager.Instance.PlaySoundEffect(soundEffect);
    }
}
