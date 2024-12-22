using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitParticle : MonoBehaviour
{
    #region Variables

    public Transform pivot;
    public UnityEvent PlaySoundEffect;
    public string hitParticleName = "HitEffect";
    #endregion
    #region UnityMethod
    public void Start()
    {
        if (pivot == null)
            pivot = transform;
    }
    #endregion
    #region MainMethod


    public async UniTaskVoid doSpawnKnockbackParticle(Vector3 KnockDirect, float StunTime = 0.15f)
    {
        GameObject prefab = ParticleManager.Instance.SpawnParticle(hitParticleName,
            pivot.position
            , Quaternion.LookRotation(KnockDirect));

        //Implement Knockback shiet here
        KnockDirect.y = 0;
        PlaySoundEffect.Invoke();
    }
    public void PlayExplodeParticle()
    {
        ParticleManager.Instance.SpawnParticle("BotExplode", pivot.position, Quaternion.Euler(0, 0, 0));
    }
    public void SpawnKnockbackParticle(Vector3 KnockDirect)
    {
        doSpawnKnockbackParticle(KnockDirect);
    }
    #endregion
    #region Ults

    [FoldoutGroup("Event Test")]
    [Button]

    public void PlayParticle()
    {
        PlaySoundEffect.Invoke();
    }


    #endregion

}
