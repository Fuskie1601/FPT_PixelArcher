using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Skill_Active_Striking : ISkill
{
    [FoldoutGroup("Stats/Damage")]
    public int Damage = 10;
    [FoldoutGroup("Stats/Damage")]
    public float dotTime = 0.2f, strikingDelayTime = 0.25f;
    [FoldoutGroup("Stats/Movement")]
    public float strikingMultiplier;
    [FoldoutGroup("Stats")]
    public int staminaStrikeCost;
    [FoldoutGroup("Debug")] 
    public bool allowMark;
    [FoldoutGroup("Debug")] 
    public List<Health> MarkedEnity;

    public List<TrailRenderer> trail;

    //Calculate
    private Health enemyMarked;

    private void Start()
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc._playerMovementManager.strikeMultiplier = strikingMultiplier;

        TrailEffect(false);
    }

    private void OnValidate()
    {
        if(_pc != null) _pc._playerMovementManager.strikeMultiplier = strikingMultiplier;
    }

    public override void Activate()
    {
        if (currentCD <= 0)
        {
            Debug.Log(Name + " Activated");
            Striking();
        }
    }
    
    [Button]
    public void Striking()
    {
        if (_pc.moveBuffer == Vector2.zero) return;
        doStrikingMove(_pc.moveBuffer, staminaStrikeCost);
    }
    public async UniTaskVoid doStrikingMove(Vector2 input, int staminaCost)
    {
        //prevent spam in the middle
        if (!_pc.staminaSystem.HasEnoughStamina(staminaCost) || _pc.moveBuffer == Vector2.zero) return;
        
        currentCD = Cooldown;
        _pc.currentState = PlayerState.Striking;
        //_playerAnimController.StrikingAnim();
        allowMark = true;
        _pc.gameObject.layer = LayerMask.NameToLayer("Arrow");
        
        //consume Stamina here
        _pc.staminaSystem.Consume(staminaCost);
        TrailEffect(true);

        //striking done ? okay cool
        await UniTask.Delay(TimeSpan.FromSeconds(_pc._stats.rollTime));
        _pc.currentState = PlayerState.Idle;
        allowMark = false;
        _pc.gameObject.layer = LayerMask.NameToLayer("Player");
        TrailEffect(false);
        
        //Might add some event here to activate particle or anything
        await UniTask.Delay(TimeSpan.FromSeconds(strikingDelayTime));
        StrikeDamage();
    }
    async UniTaskVoid StrikeDamage()
    {
        foreach (var enemy in MarkedEnity)
        {
            enemy.DamageOverTime(Damage, dotTime);
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(dotTime));
        MarkedEnity.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!allowMark) return;
        
        if (other.gameObject.tag == "Enemy")
        {
            //Debug.Log(other.gameObject.name);
            enemyMarked = other.GetComponent<Health>();
            if (enemyMarked != null)
            {
                MarkedEnity.Add(enemyMarked);
                Debug.Log("marked " + enemyMarked.gameObject.name);
            }
        }
    }

    void TrailEffect(bool toggle)
    {
        foreach (var t in trail)
        {
            if(t != null)
            t.emitting = toggle;
        }
    }
}
