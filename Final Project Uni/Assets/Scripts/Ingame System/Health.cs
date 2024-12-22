using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum HealthState
{
    Idle, Invincible, Absorbtion, Survivor, Parry, Def
}

public class Health : MonoBehaviour
{
    #region Variables

    [FoldoutGroup("Stats")] 
    public bool isPlayer;
    [FoldoutGroup("Stats")]
    public HealthState healthState;
    [FoldoutGroup("Stats")]
    public int maxHealth;
    [FoldoutGroup("Stats")]
    [ReadOnly, SerializeField] public int health, overHeal;

    [FoldoutGroup("Stats/Buff")] 
    public bool isTrapMaster = false;


    [FoldoutGroup("Setup")]
    [SerializeField, CanBeNull] private StatSliderUI StatUI;
    [FoldoutGroup("Setup")]
    public bool fillOnStart = true;
    [FoldoutGroup("Setup/Event")]
    public UnityEvent HpValueChange, HpReduce, OnDeath, OnRevive;
    [FoldoutGroup("Setup/Event")]
    public UnityEvent<Vector3> OnKnockback;
    [FoldoutGroup("Setup/Event")]
    public UnityEvent<int, int> CheckValue;

    [FoldoutGroup("Debug")] public bool isAlive = true;
    [FoldoutGroup("Debug")] public Vector3 KnockDir;

    //Calculate
    private int Damage;
    public int currentHealth
    {
        get { return health; }
        set
        {
            int previousHealth = health; // Save the current health for comparison
            if (value > maxHealth)
            {
                this.overHeal = value - maxHealth;
                value = maxHealth;
            }
            this.health = value;
        
            // Call DeathEvent if the character is newly dead
            if (value <= 0 && isAlive)
            {
                DeathEvent();
            }
        
            // Force an update if health changes or if health is set to the same value
            if (previousHealth != health || value == maxHealth) 
            {
                HPUpdate();
            }
        }
    }
    //use this

    #endregion

    #region Unity Method

    private void Awake()
    {
        if (TryGetComponent<StatSliderUI>(out StatSliderUI ui)) StatUI = ui;
    }

    private void Start()
    {
        if (fillOnStart) currentHealth = maxHealth;
    }

    #endregion

    #region Event

    public void HPUpdate()
    {
        //Debug.Log(this.gameObject.name + " HP Update");
        if (StatUI != null) StatUI.UpdateValue(health, maxHealth);
        HpValueChange.Invoke();
    }
    public void DeathEvent()
    {
        Debug.Log(this.gameObject.name + " Dead");
        isAlive = false;
        OnDeath.Invoke();
    }

    #endregion

    #region Ults

    [FoldoutGroup("Event Test")]
    [Button]
    public void Invincible(float time)
    {
        InvincibleTimer(time);
    }

    [FoldoutGroup("Event Test")]
    [Button]
    public void Absorbtion(float time)
    {
        AbsorbtionTimer(time);
    }

    [FoldoutGroup("Event Test/Basic")]
    [Button]
    public void Hurt(int damage)
    {
        switch (healthState)
        {
            case HealthState.Idle:
                DealDamage(damage);
                break;
            case HealthState.Invincible:
                break;
            case HealthState.Absorbtion:
                Heal(damage);
                break;
            case HealthState.Survivor:
                DealDamageSurvivor(damage);
                break;
            case HealthState.Parry:
                DealDamage(0);
                break;
            case HealthState.Def:
                damage = Mathf.RoundToInt(damage * 0.5f);
                if (damage <= 1) damage = 0;
                
                DealDamage(damage);
                break;

        }
    }

    [FoldoutGroup("Event Test/Extend")]
    [Button]
    public void DamageOverTime(int damage, float time)
    {
        Debug.Log("Damage Over Time: " + damage + " in " + time + " secs");
        DoT(damage, time);
    }


    [FoldoutGroup("Event Test/Basic")]
    [Button]
    public void Heal(int heal)
    {
        Debug.Log("Heal " + heal);
        currentHealth += heal;
    }
    
    [FoldoutGroup("Event Test/Basic")]
    [Button]
    public void HealPercent(float healPercent)
    {
        int heal = Mathf.CeilToInt(maxHealth * healPercent);
        Debug.Log("Heal " + heal);
        currentHealth += heal;
    }

    [FoldoutGroup("Event Test/Extend")]
    [Button]
    public void HealOverTime(int heal, float time)
    {
        //Debug.Log("Heal Over Time: " + heal + " in " + time + " secs");
        HoT(heal, time);
    }
    
    [Button]
    public void FullHeal(float multiply = 1)
    {
        int targetHealth = Mathf.RoundToInt(maxHealth * multiply);
        //Debug.Log($"FullHeal: Setting health to {targetHealth} (maxHealth: {maxHealth}, multiply: {multiply})");
        currentHealth = targetHealth; // Use the property to ensure proper behavior
    }


    [FoldoutGroup("Event Test/Basic")]
    [Button]
    public void Knockback(Vector3 Dir, float knockForce = 10)
    {
        if (!isPlayer && Dir == Vector3.zero) Dir = -transform.forward.normalized;
        Dir.y = 0;
        OnKnockback.Invoke(Dir.normalized * knockForce);
    }


    private async UniTaskVoid InvincibleTimer(float time)
    {
        healthState = HealthState.Invincible;
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        healthState = HealthState.Idle;
    }

    private async UniTaskVoid AbsorbtionTimer(float time)
    {
        healthState = HealthState.Absorbtion;
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        healthState = HealthState.Idle;
    }

    void DealDamageSurvivor(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) currentHealth = 1;
        HpReduce.Invoke();
    }
    void DealDamage(int damage)
    {
        if(!isAlive) return;
        //Debug.Log("Received " + damage);
        currentHealth -= damage;
        //if (currentHealth <= 0) currentHealth = -1;
        HpReduce.Invoke();
    }
    async UniTaskVoid DoT(int damage, float duration)
    {
        float timePerTick = duration / damage; // Calculate the time between each damage tick
        int ticks = damage; // The number of times to apply damage
        for (int i = 0; i < ticks; i++)
        {
            Hurt(1); // Call Hurt with 1 damage for each tick
            if (currentHealth <= 0) break; // Exit the loop if the character is dead
            await UniTask.Delay(TimeSpan.FromSeconds(timePerTick)); // Wait before the next tick
        }
    }
    async UniTaskVoid HoT(int heal, float duration)
    {
        float timePerTick = duration / heal; // Calculate the time between each damage tick
        int ticks = heal; // The number of times to apply damage
        for (int i = 0; i < ticks; i++)
        {
            Heal(1); // Call Hurt with 1 damage for each tick
            if (currentHealth <= 0) break; // Exit the loop if the character is dead
            await UniTask.Delay(TimeSpan.FromSeconds(timePerTick)); // Wait before the next tick
        }
    }

    public void Parry(float timer = 0.1f)
    {
        doParryState(timer);
    }
    
    async void doParryState(float timer)
    {
        healthState = HealthState.Parry;
        await UniTask.Delay(TimeSpan.FromSeconds(timer));

        if (healthState == HealthState.Parry) healthState = HealthState.Idle;
    }
    
    public void DefState(float timer = 0.25f)
    {
        doDefState(timer);
    }

    async void doDefState(float timer)
    {
        healthState = HealthState.Def;
        await UniTask.Delay(TimeSpan.FromSeconds(timer));

        if (healthState == HealthState.Def) healthState = HealthState.Idle;
    }

    public void IdleState()
    {
        healthState = HealthState.Idle;
    }

    #endregion
}
