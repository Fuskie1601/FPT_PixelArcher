using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ArrowController : MonoBehaviour, IWeapon
{
    #region Variables

    public StatSliderUI _StatSliderUI;

    [FoldoutGroup("Stats")]
    public AnimationCurve forceCurve;

    [FoldoutGroup("Stats")]
    public float ShootForce, currentChargedTime, chargedTime = 2f;

    #region Debug and Setup

    [FoldoutGroup("Debug")]
    public bool blockInput, allowRecall = true;
    [FoldoutGroup("Debug")]
    public List<Arrow> arrowsList;
    [FoldoutGroup("Debug")]
    public PhysicMaterial physicMat;
    
    [FoldoutGroup("Debug/States")]
    [ReadOnly] public bool ChargingInput;
    [FoldoutGroup("Debug/States")]
    public bool FullyCharged, isRecalling, RecallBuffer, arrowRecoverFlag, haveArrow;
    [FoldoutGroup("Debug/States")]
    public bool ShootButtonPressing;

    [FoldoutGroup("Debug/Setup")] public GameObject ArrowPrefab;
    [FoldoutGroup("Debug/Setup")] public ParticleManager prefabParticleManager;
    [FoldoutGroup("Debug/Setup")] public Arrow MainArrow;
    [FoldoutGroup("Debug/UI")] public Image ShootIcon;
    [FoldoutGroup("Debug/UI")] public Sprite ShootSprite, RecallSprite;
    [FoldoutGroup("Debug/Buff")] public bool IsSplitShot = false;
    [FoldoutGroup("Debug/Buff")] public bool IsPreciseArcher = false;
    [FoldoutGroup("Debug/Buff")] public bool IsExplosionExpert = false;

    public static ArrowController Instance;
    private PlayerController _playerController;
    [HideInInspector] public PlayerAnimManager playerAnimManager;
    private float calForce;
    #endregion

    #endregion

    #region Unity Event

    private void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (var arrow in arrowsList)
        {
            arrow.Assign();
        }
    }
    private void Start()
    {
        _playerController = PlayerController.Instance;
        playerAnimManager = _playerController.playerAnimManager;

        haveArrow = true;
        ShootSpriteUpdate();
        playerAnimManager.UpdateHaveArrow(true);
    }

    private void Update()
    {
        _StatSliderUI.UpdateValue(currentChargedTime, chargedTime);
            
        UpdateCharging();

        if (RecallBuffer)
        {
            RecallArrow(MainArrow);
            
            if(!IsSplitShot) return;
            foreach (var arrow in arrowsList)
            {
                if (!arrow.IsMainArrow) RecallArrow(arrow);
            }
        }

    }

    #endregion

    #region Input

    public void ChargeShoot(InputAction.CallbackContext ctx)
    {
        ChargeShoot(ctx.performed);
    }
    public void Recall(InputAction.CallbackContext ctx)
    {
        Recall(ctx.performed);
    }

    //Mobile Input
    public void ChargeShoot(bool charge)
    {
        if (blockInput) return;
        //have arrow and alive ? cool
        if (!haveArrow || !_playerController.PlayerHealth.isAlive) return;
        ChargingInput = charge;
        
        playerAnimManager.Draw(true, true);
    }
    public void Recall(bool recall)
    {
        if (blockInput || !allowRecall) return;
        
        ShootButtonPressing = recall;
        if (haveArrow || !_playerController.PlayerHealth.isAlive) return;
        isRecalling = recall;
        StartRecall(recall);
    }

    #endregion

    #region Shoot

    void UpdateCharging()
    {
        if (ChargingInput)
        {
            if(!_StatSliderUI.toggleShow) _StatSliderUI.UpdateToggle(true);
            
            if (currentChargedTime <= chargedTime)
            {
                currentChargedTime += Time.deltaTime;
                //Debug.Log(currentChargedTime);
            }
        }
    }
    void ShootArrow(Arrow arrow)
    {
        //Stat adjust
        if (IsPreciseArcher && (currentChargedTime / chargedTime) >= 0.75f)
            _playerController._stats.SetBuffATKMul(0.5f);
        else
            _playerController._stats.SetBuffATKMul(0);
        

        calForce = forceCurve.Evaluate(currentChargedTime / chargedTime) * ShootForce;

        // Calculate the rotation around the Y-axis based on the offset in degrees
        Quaternion rotationOffset = Quaternion.Euler(0, arrow.offsetDegree, 0);

        // Apply the rotation offset to the player's forward direction (ignore Y-axis for shooting)
        Vector3 shootDir = rotationOffset * _playerController.transform.forward;
        shootDir.y = 0; // Ensure no vertical component in the shooting direction

        // Activate the arrow game object
        arrow.gameObject.SetActive(true);
        arrow.transform.position = _playerController.transform.position;

        // Set the rotation so the Z-axis points in the shoot direction
        arrow.transform.rotation = Quaternion.LookRotation(shootDir.normalized);

        // Shoot the arrow with the calculated force
        arrow.Shoot(shootDir.normalized * calForce);
        arrow.currentArrowState = ArrowState.Shooting;
    }


    [Button]
    public void Shoot()
    {
        if (blockInput) return;
        if (!haveArrow || arrowRecoverFlag)
        {
            arrowRecoverFlag = false;
            return;
        }

        //because button up
        ChargingInput = false;
        haveArrow = false;
        ShootSpriteUpdate();
        playerAnimManager.UpdateHaveArrow(haveArrow);
        ShootArrow(MainArrow);
        if (IsSplitShot)
        {
            foreach (var arrow in arrowsList)
            {
                if(!arrow.IsMainArrow) ShootArrow(arrow);
            }
        }
        AudioManager.Instance.PlaySound("ShootArrow");
        //wear
        playerAnimManager.Draw(false, true);
        currentChargedTime = 0;
        _StatSliderUI.UpdateToggle(false, 0.5f);
    }

    #endregion

    #region Recall
    public async UniTaskVoid HideAllMirageArrow(float time = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        foreach (var arrow in arrowsList)
        {
            if(!arrow.IsMainArrow) arrow.HideArrow();
        }
    }
    void RecallArrow(Arrow arrow)
    {
        if (arrow == null) return;
        if (arrow.currentArrowState == ArrowState.Shooting)
        {
            RecallBuffer = true;
            //return;
        }

        if (isRecalling)
        {
            RecallBuffer = false;
            
            //if (arrow.currentArrowState != ArrowState.Shooting)
            {
                arrow.StartRecallEvent.Invoke();
                
                arrow.currentArrowState = ArrowState.Recalling;
                if (arrow.IsMainArrow && arrow.currentArrowState == ArrowState.Recalling)
                    prefabParticleManager.PlayAssignedParticle("RecallingMainArrowVFX");
            }
        }
        else
        {
            arrow.StopRecallEvent.Invoke();
            arrow.currentArrowState = ArrowState.Idle;
        }
    }
    [Button]
    public void StartRecall(bool recallBool)
    {
        isRecalling = recallBool;
        if (!_playerController.PlayerHealth.isAlive || _playerController.currentState == PlayerState.Stunning || haveArrow) return;

        if (recallBool)
        {
            _playerController.currentState = PlayerState.Recalling;
            playerAnimManager.RecallAnimTrigger();
        }
        else 
            _playerController.currentState = PlayerState.Idle;
        
        RecallArrow(MainArrow);
        if (!IsSplitShot) return;
        
        foreach (var arrow in arrowsList)
        {
            RecallArrow(arrow);
        }

    }
    #endregion

    #region Remote Recover

    public void RemoteRecover()
    {
        if (blockInput) return;
        if (haveArrow) return;
        
        Debug.Log("Remote Recover");
        prefabParticleManager.PlayAssignedParticle("RecallingMainArrowVFX");
        foreach (var arrow in arrowsList)
        {
            arrow.HideArrow(true);
        }

        haveArrow = true;
        ShootSpriteUpdate();
        isRecalling = false;
    }

    #endregion

    #region UI Update (not recommended do this, Im just lazy, lol)

    public void ShootSpriteUpdate()
    {
        if(ShootIcon == null || RecallSprite == null) return;

        if (haveArrow) ShootIcon.sprite = ShootSprite;
        else ShootIcon.sprite = RecallSprite;
    }
    
    #endregion
}
