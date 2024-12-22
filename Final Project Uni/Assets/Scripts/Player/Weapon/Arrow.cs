using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum ArrowState
{
    Idle, Shooting, Recalling
}

public class Arrow : MonoBehaviour
{
    public ArrowState currentArrowState;

    [FoldoutGroup("Stats")]
    public Vector3 AccelDirect;
    [FoldoutGroup("Stats")]
    public float lifeTime, recallSpeed, rotSpeed = 10, MaxSpeed, minShootingSpeed = 0.5f, MirageDelay = 0.5f;

    [FoldoutGroup("Stats")]
    public float bonusRicochetMultiplier = 0;
    [FoldoutGroup("Stats/Hover")]
    public float hoverSpeed = 2.0f;

    [FoldoutGroup("Debug")]
    [ReadOnly] public float CurrentVelocity;
    [FoldoutGroup("Debug")]
    public Vector3 RecallDirect;
    [FoldoutGroup("Debug/Hover")]
    public float currentHoverHeight;

    [FoldoutGroup("Setup")]
    public bool IsMainArrow, startRecallFlag;
    [FoldoutGroup("Setup")]
    public float offsetDegree;
    [FoldoutGroup("Setup")]
    public Rigidbody arrowRb;
    [FoldoutGroup("Setup")]
    public HurtBox hitbox;
    [FoldoutGroup("Setup")]
    [ReadOnly] public ArrowController _arrowController;
    [FoldoutGroup("Setup/Events")]
    public UnityEvent StartRecallEvent, StopRecallEvent, RecoverEvent, HideEvent, RemoteRecoverEvent;
    [FoldoutGroup("Setup/Skill")] 
    public bool isExplodeOnRemote;
    [FoldoutGroup("Setup/Skill")] 
    [CanBeNull] public GameObject MiniExplode;
    
    
    bool hasBuffBeenReset = false;

    #region Unity Methods

    public void OnEnable()
    {
        hitbox.KnockDir = arrowRb.velocity;
    }
    private void Start()
    {
        _arrowController = ArrowController.Instance;

        RecoverEvent.Invoke();
        if (!IsMainArrow) hitbox.MirageMultiplier = 0.5f;
    }

    private void FixedUpdate()
    {
        CurrentVelocity = arrowRb.velocity.magnitude;
    }

    private void Update()
    {
        startRecallFlag = (currentArrowState == ArrowState.Shooting);

        // Ensure SetBuffATKMul(0) is called only once
        if (!startRecallFlag && !hasBuffBeenReset)
        {
            PlayerController.Instance._stats.SetBuffATKMul(0);
            hasBuffBeenReset = true; // Set the flag to true after the method is called
        }

        // Reset the flag when the arrow starts shooting again
        if (startRecallFlag)
        {
            hasBuffBeenReset = false;
        }

        if (currentArrowState == ArrowState.Shooting && arrowRb.velocity.magnitude <= minShootingSpeed)
        {
            currentArrowState = ArrowState.Idle;
        }

        if (currentArrowState == ArrowState.Recalling) Recall();

        else if (currentArrowState == ArrowState.Idle)
        {
            currentHoverHeight = 0;

            if (IsMainArrow && currentArrowState == ArrowState.Recalling)
                _arrowController.prefabParticleManager.PlayAssignedParticle("RecallingMainArrowVFX");
        }

        // Rotate the arrow to point in the direction of its velocity
        if (arrowRb.velocity.magnitude > 0 && arrowRb.velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(arrowRb.velocity.normalized);
            arrowRb.rotation = Quaternion.Slerp(arrowRb.rotation, targetRotation, Time.deltaTime * rotSpeed);
        }
    }


    private void OnDisable()
    {
        //HideEvent.Invoke();

        _arrowController.prefabParticleManager.SpawnParticle("ArrowHideVFX",
            transform.position, Quaternion.Euler(-90, 0, 0));
    }

    #endregion
    public void Assign()
    {
        arrowRb = GetComponent<Rigidbody>();
        hitbox = GetComponent<HurtBox>();
    }


    public void Shoot(Vector3 inputDirect)
    {
        AccelDirect = inputDirect;
        //Debug.Log(AccelDirect);
        arrowRb.velocity = AccelDirect;
        hitbox.KnockDir = arrowRb.velocity;
    }

    #region Recalling

    public void Recall()
    {
        RecallDirect = PlayerController.Instance.transform.position - transform.position;
        //arrowRb.AddForce(RecallDirect.normalized * (recallSpeed * Time.fixedDeltaTime * 240), ForceMode.Acceleration);
        
        arrowRb.AddForce(RecallDirect.normalized * (recallSpeed * Time.deltaTime * 300f), ForceMode.Acceleration);

        LimitSpeed();
    }


    #endregion

    void LimitSpeed()
    {
        Mathf.Clamp(arrowRb.velocity.magnitude, 0, MaxSpeed);
        if (arrowRb.velocity.magnitude > MaxSpeed)
            arrowRb.velocity = arrowRb.velocity.normalized * MaxSpeed;
    }

    private void OnTriggerStay(Collider other)
    {
        //hit anything -> allow recall
        if (currentArrowState == ArrowState.Shooting) return;
        if (other.gameObject.tag == "Player")
        {
            if (IsMainArrow)
            {
                PlayerController.Instance.currentState = PlayerState.Idle;
                _arrowController.haveArrow = true;
                _arrowController.ShootSpriteUpdate();
                _arrowController.playerAnimManager.UpdateHaveArrow(true);
                _arrowController.isRecalling = false;

                if (_arrowController.ShootButtonPressing)
                    _arrowController.arrowRecoverFlag = true;

                if (_arrowController.IsSplitShot)
                    _arrowController.HideAllMirageArrow(MirageDelay);
                currentArrowState = ArrowState.Idle;
                AudioManager.Instance.PlaySound("RecallArrow");
            }
            HideArrow();
        }
    }
    public void HideArrow(bool isRemote = false)
    {
        //can Play an Event here
        if (!isRemote)
            RecoverEvent.Invoke();
        else
            RemoteRecoverEvent.Invoke();

        //hide and deactivate it
        if (!gameObject.activeSelf) return;

        arrowRb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        //hit anything -> allow recall
        if (currentArrowState == ArrowState.Shooting) currentArrowState = ArrowState.Idle;

        arrowRb.velocity = arrowRb.velocity * (0.5f + bonusRicochetMultiplier);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            _arrowController.haveArrow = false;
    }

    public void ExplosionArrow()
    {
        if(!isExplodeOnRemote) return;
        PlayerController.Instance.staminaSystem.Consume(10);
        if (MiniExplode != null)
            PoolManager.Instance.Spawn(MiniExplode, transform.position + Vector3.up, Quaternion.identity, 2f);
    }
    
    public void DebugTest(string test)
    {
        Debug.Log(test);
    }
}
