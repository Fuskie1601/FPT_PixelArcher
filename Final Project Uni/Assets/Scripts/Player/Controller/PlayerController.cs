using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public enum PlayerState
{
    Idle, Running, Recalling, ReverseRecalling, Stunning, Rolling, Striking, Guard
}

public class PlayerController : MonoBehaviour
{
    #region Variables

    [FoldoutGroup("Stats")]
    public bool blockInput;
    [FoldoutGroup("Stats")]
    public Health PlayerHealth;
    [FoldoutGroup("Stats/Skill")]
    public bool isSonicDash, haveAura;


    [FoldoutGroup("Debug")] 
    public Rigidbody PlayerRB;
    [FoldoutGroup("Debug")]
    public PlayerState currentState;
    [FoldoutGroup("Debug")]
    [ReadOnly] public Vector2 moveInput = Vector2.zero, moveBuffer;
    [FoldoutGroup("Debug")]
    public Vector2 joyStickInput;
    [FoldoutGroup("Debug")]
    public bool isJoystickInput;

    [FoldoutGroup("Setup/UI")] 
    public UIContainer _UIContainer;
    [FoldoutGroup("Setup/UI")] 
    public DialogUI dialogUI;
    [FoldoutGroup("Setup/UI")] 
    public Button interactButton;
    [FoldoutGroup("Setup/UI")] 
    [CanBeNull] public NotifAnim notif;
    [FoldoutGroup("Setup/UI")] 
    [CanBeNull] public List<TMP_Text> GoldAmount, SoulAmount;
    [FoldoutGroup("Setup/UI")]
    public UltimateJoystick JoystickPA;
    
    [FoldoutGroup("Setup")]
    public PlayerMovementManager _playerMovementManager;
    [FoldoutGroup("Setup")]
    public PlayerAnimManager playerAnimManager;
    [FoldoutGroup("Setup")]
    public ArrowController _arrowController;
    
    [FoldoutGroup("Setup")]
    [CanBeNull] public HitStop _hitStop;
    [FoldoutGroup("Setup")] 
    public StaminaSystem staminaSystem;
    [FoldoutGroup("Setup")] 
    public SeeThroughDetector xrayDetector;

    [FoldoutGroup("Setup/Save")]
    public PlayerProgressData PlayerProgressData;
    [FoldoutGroup("Setup/Save")]
    public PlayerStats _stats;


    #region Calculate

    public static PlayerController Instance;

    //Calculate
    private Quaternion targetRotation;
    private float closestDistance, elapsedTime;
    
    #endregion

    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (xrayDetector != null) xrayDetector.Activate = true;
        }
        if (PlayerHealth == null) PlayerHealth = GetComponent<Health>();
        PlayerHealth.isPlayer = true;
        if (PlayerRB == null) PlayerRB = GetComponent<Rigidbody>();
        
        
        interactButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (Instance == null) Instance = this;
    }

    private void FixedUpdate()
    {
        JoyStickInput();
        SpeedCheck();
    }

    private void Update()
    {
        _arrowController.blockInput = blockInput;
        if (blockInput) return;

        _playerMovementManager.UpdateRollCDTimer();

        _playerMovementManager.Move(moveInput);
        UpdateAnimState();
        _playerMovementManager.RotatePlayer(_playerMovementManager.moveDirection, _stats.rotationSpeed);
        _playerMovementManager.RollApply();
        _playerMovementManager.StrikingMoveApply(_playerMovementManager.strikeMultiplier);
        _playerMovementManager.ReverseRecall();
    }

    #endregion
    
    #region Input

    public void InputMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput != Vector2.zero && moveInput != moveBuffer) moveBuffer = moveInput;
    }

    public void InputRoll(InputAction.CallbackContext ctx)
    {
        _playerMovementManager.Roll();
    }

    void JoyStickInput()
    {
        joyStickInput.x = JoystickPA.GetHorizontalAxis();
        joyStickInput.y = JoystickPA.GetVerticalAxis();

        isJoystickInput = (joyStickInput != Vector2.zero);

        if (joyStickInput != Vector2.zero && joyStickInput != moveBuffer)
            moveBuffer = joyStickInput;
    }

    #endregion

    #region Animation

    void UpdateAnimState()
    {
        if (currentState == PlayerState.Stunning || currentState == PlayerState.Rolling || 
            currentState == PlayerState.Striking || currentState == PlayerState.Guard) goto Skip;
        if (currentState == PlayerState.ReverseRecalling) goto ReverseRecallFlag;

        currentState = PlayerState.Idle;
        if (_playerMovementManager.moveDirection != Vector3.zero) currentState = PlayerState.Running;
        playerAnimManager.UpdateRunInput(currentState == PlayerState.Running);
        if (_arrowController.isRecalling) currentState = PlayerState.Recalling;
        playerAnimManager.RecallAnim(_arrowController.isRecalling);

    ReverseRecallFlag:
        if (currentState == PlayerState.ReverseRecalling)
        {
            playerAnimManager.RecallAnim(true, true);
            playerAnimManager.RecallAnimTrigger();
        }

    Skip:;
    }

    

    public void MeleeAnim()
    {
        if (blockInput || !PlayerHealth.isAlive) return;
        if (currentState == PlayerState.Recalling || currentState == PlayerState.ReverseRecalling || currentState == PlayerState.Stunning) return;
        if (_arrowController.ChargingInput) return;
        playerAnimManager.Slash();
    }


    #endregion
    
    #region Event
    
    public void HurtAnim()
    {
        playerAnimManager.DamagedAnim();
    }

    public void ReceiveKnockback(Vector3 KnockDirect)
    {
        doReceiveKnockback(KnockDirect);
    }

    public async UniTaskVoid doReceiveKnockback(Vector3 KnockDirect, float StunTime = 0.15f)
    {
        if(!PlayerHealth.isAlive) return;

        _playerMovementManager.Knockback(KnockDirect);

        currentState = PlayerState.Stunning;
        await UniTask.Delay(TimeSpan.FromSeconds(StunTime));
        currentState = PlayerState.Idle;
    }

    public void Die()
    {
        Debug.Log("Ded");
        playerAnimManager.DieAnim(true);
        PlayerHealth.isAlive = false;
    }

    public void Revive(float InstantHPPercent = 0.5f, int RegenHP = 40)
    {
        playerAnimManager.DieAnim(false);
        PlayerHealth.FullHeal(InstantHPPercent);
        _UIContainer.GameplayState();
        PlayerHealth.HealOverTime(RegenHP, 4);
        PlayerHealth.isAlive = true;
        PlayerHealth.Invincible(2);
        
        PlayerHealth.OnRevive.Invoke();
    }

    #endregion

    #region Debug

    void SpeedCheck()
    {
        _playerMovementManager.currentAccel = _playerMovementManager.PlayerRB.velocity.magnitude;
    }

    [FoldoutGroup("Debug")]
    [Button]
    public void forceChangeState(PlayerState state)
    {
        currentState = state;
    }

    #endregion

    #region UI (temp)

    public void UpdateUI(string Gold, string Soul)
    {
        foreach (var goldText in GoldAmount)
        {
            if (goldText.text != null) goldText.text = Gold;
        }

        foreach (var soulText in SoulAmount)
        {
            if (soulText.text != null) soulText.text = Soul;
        }
    }

    public void ResetStat()
    {
        foreach (var skillobj in SkillHolder.Instance.skillList)
        {
            Destroy(skillobj);
        }
        
        SkillHolder.Instance.skillList.Clear();
        SkillHolder.Instance.SkillIDList.Clear();
        SkillHolder.Instance.SkillOBJNameList.Clear();
        if (SkillHolder.Instance.currentSkillUISprite != null) 
            SkillHolder.Instance.currentSkillUISprite.sprite = SkillHolder.Instance.defaultUISprite;

        _stats.ResetBonus();

        isSonicDash = false;
        haveAura = false;
    }

    #endregion
}
