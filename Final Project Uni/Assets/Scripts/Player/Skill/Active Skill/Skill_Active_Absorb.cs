using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class Skill_Active_Absorb : ISkill
{
    [FoldoutGroup("Stats")]
    public float AbsorbTime = 5;
    [CanBeNull] public GameObject effect;

    private void Start()
    {
        if(_pc == null) _pc = PlayerController.Instance;
    }

    public override void Activate()
    {
        if (currentCD <= 0)
        {
            Debug.Log(Name + " Activated");
            AbsorbBuff();
            currentCD = Cooldown;
        }
    }
    
    [Button]
    public async UniTaskVoid AbsorbBuff()
    {
        if(effect != null) effect.SetActive(true);
        _pc.PlayerHealth.Absorbtion(AbsorbTime);

        await UniTask.Delay(TimeSpan.FromSeconds(AbsorbTime));
        if(effect != null) effect.SetActive(false);
    }

}
