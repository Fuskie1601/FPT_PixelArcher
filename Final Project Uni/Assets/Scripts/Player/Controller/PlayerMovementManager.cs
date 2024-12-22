using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovementManager : MonoBehaviour
{
    public PlayerController pController;
    public Rigidbody PlayerRB;

    [FoldoutGroup("Stats")]
    public bool blockInput;
    [FoldoutGroup("Stats")]
    public Health PlayerHealth;
    [FoldoutGroup("Stats/Lunge")]
    public float meleeLungeForce = 100f, lungeAngle = 60f, rangeAngleTolerance = 5f, lungeRange = 25f, LungeTime = 0.3f;

    [FoldoutGroup("Stats/Skill")]
    public GameObject SonicBoomPrefab;

    [FoldoutGroup("Debug")]
    [SerializeField, ReadOnly] public float currentAccel;
    [FoldoutGroup("Debug/Roll")]
    [SerializeField, ReadOnly] private bool canRoll;
    [FoldoutGroup("Debug/Roll")]
    [SerializeField, ReadOnly] private float currentRollCD;
    [FoldoutGroup("Debug/Roll")]
    [SerializeField, ReadOnly] private Vector3 RollDirect;
    [FoldoutGroup("Debug/Striking")]
    [SerializeField, ReadOnly] public float strikeMultiplier = 1.75f;
    [FoldoutGroup("Debug/Reverse Recall")]
    [SerializeField, ReadOnly] public float ReverseRecallMultiplier = 1;

    public UnityEvent StartDodge;

    private Vector3 rollDir;
    [HideInInspector] public Vector3 calculateMove, moveDirection;
    private Vector3 cameraForward, cameraRight;
    private Vector3 RecallDirect, lungeDirection;
    private Quaternion targetRotation, leftRayRotation, rightRayRotation;
    private Vector3 leftRayDirection, rightRayDirection;

    private GameObject closestEnemy;
    private float closestDistance, elapsedTime;

    #region Calculate

    public void UpdateRollCDTimer()
    {
        if (currentRollCD > 0)
            currentRollCD -= Time.deltaTime;
        else
            canRoll = true;
    }
    void AddRollCD(float time)
    {
        currentRollCD += time;
    }

    public async UniTaskVoid doRollingMove(Vector2 input, int staminaCost)
    {
        //prevent spam in the middle
        if (!canRoll || !pController.staminaSystem.HasEnoughStamina(staminaCost) ||
            pController.moveBuffer == Vector2.zero || !PlayerHealth.isAlive) return;
        AudioManager.Instance.PlaySound("Dash");

        //add CD
        AddRollCD(pController._stats.rollCD + pController._stats.rollTime);
        canRoll = false; //just want to save calculate so I place here, hehe

        //this lead to the Roll Apply
        rollDir = transform.forward.normalized;
        pController.currentState = PlayerState.Rolling;
        pController.playerAnimManager.DodgeAnim();
        StartDodge.Invoke();

        pController._arrowController.isRecalling = false;

        //consume Stamina here
        pController.staminaSystem.Consume(staminaCost);

        //roll done ? okay cool
        if (pController.haveAura)
        {
            pController.PlayerHealth.healthState = HealthState.Parry;
            ParticleManager.Instance.PlayAssignedParticle("ParryAura");
        }

        await UniTask.Delay(TimeSpan.FromSeconds(pController._stats.rollTime * 0.5f));
        if (pController.haveAura)
        {
            pController.PlayerHealth.healthState = HealthState.Idle;
            ParticleManager.Instance.StopAssignedParticle("ParryAura");
        }

        await UniTask.Delay(TimeSpan.FromSeconds(pController._stats.rollTime * 0.5f));
        pController.currentState = PlayerState.Idle;
        //Might add some event here to activate particle or anything
    }

    public void RollApply()
    {
        if (pController.currentState == PlayerState.Rolling)
        {
            // Take from buffer
            moveDirection = (cameraRight * pController.moveBuffer.x + cameraForward * pController.moveBuffer.y).normalized;

            // Mix the directions
            Vector3 ControlRoll = Vector3.Lerp(transform.forward.normalized, moveDirection, 0.2f).normalized;
            Vector3 mixedDirection = Vector3.Lerp(rollDir, ControlRoll, pController._stats.controlRollDirect).normalized;

            // Implement Roll Logic here
            RollDirect.x = mixedDirection.x;
            RollDirect.z = mixedDirection.z;

            PlayerRB.velocity = RollDirect.normalized * (pController._stats.rollSpeed * Time.fixedDeltaTime * 240);
        }
    }

    public void StrikingMoveApply(float speedMultiplier = 2)
    {
        if (pController.currentState == PlayerState.Striking)
        {
            // Take from buffer
            moveDirection = (cameraRight * pController.moveBuffer.x + cameraForward * pController.moveBuffer.y).normalized;

            // Mix the directions
            Vector3 mixedDirection = Vector3.Lerp(transform.forward.normalized, moveDirection, pController._stats.controlRollDirect).normalized;

            // Implement Roll Logic here
            RollDirect.x = mixedDirection.x;
            RollDirect.z = mixedDirection.z;

            PlayerRB.velocity = RollDirect.normalized * (pController._stats.rollSpeed * speedMultiplier * Time.fixedDeltaTime * 240);
        }
    }

    public void SonicBoom()
    {
        if (!pController.isSonicDash) return;

        PoolManager.Instance.Spawn(SonicBoomPrefab, PlayerRB.transform.position, Quaternion.identity, 1.25f);
    }

    void LimitSpeed(float MaxSpeed)
    {
        Mathf.Clamp(PlayerRB.velocity.magnitude, 0, MaxSpeed);
        if (PlayerRB.velocity.magnitude > MaxSpeed)
            PlayerRB.velocity = PlayerRB.velocity.normalized * MaxSpeed;
    }

    #endregion

    #region Movement

    public void Move(Vector2 input = default)
    {
        if (pController.currentState == PlayerState.Stunning || pController.currentState == PlayerState.Rolling ||
            pController.currentState == PlayerState.Recalling || pController.currentState == PlayerState.ReverseRecalling ||
            pController.currentState == PlayerState.Striking || pController.currentState == PlayerState.Guard ||
            !PlayerHealth.isAlive) return;

        if (pController.isJoystickInput) input = pController.joyStickInput;

        // Calculate camera forward direction
        cameraForward = Camera.main.transform.forward.normalized;
        cameraRight = Camera.main.transform.right.normalized;

        // Calculate the move direction based on input and camera orientation

        moveDirection = (cameraRight * input.x + cameraForward * input.y).normalized;
        moveDirection.y = 0;

        // Move the Rigidbody
        if (pController.currentState != PlayerState.Rolling)
            PlayerRB.AddForce(moveDirection * (Time.deltaTime * 240 * pController._stats.speed), ForceMode.VelocityChange);

        LimitSpeed(pController._stats.maxSpeed);
    }

    public void RotatePlayer(Vector3 moveDirection, float rotationSpeed)
    {
        if (moveDirection == Vector3.zero) return;
        targetRotation = Quaternion.LookRotation(moveDirection);
        // Create a new rotation with X set to 0
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        PlayerRB.rotation = Quaternion.Slerp(PlayerRB.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Knockback(Vector3 KnockDirect)
    {
        KnockDirect.y = 0;
        PlayerRB.AddForce(KnockDirect.normalized * KnockDirect.magnitude, ForceMode.Impulse);
    }

    #endregion

    #region Special Move

    public void Roll()
    {
        if (!PlayerHealth.isAlive || blockInput || pController.currentState == PlayerState.Guard) return;
        if (pController.currentState == PlayerState.Rolling || pController.moveBuffer == Vector2.zero) return;
        doRollingMove(pController.moveBuffer, pController._stats.staminaRollCost);
    }

    public void ReverseRecall()
    {
        if (pController.currentState != PlayerState.ReverseRecalling || !pController._arrowController.MainArrow.isActiveAndEnabled) return;
        RecallDirect = pController._arrowController.MainArrow.transform.position - transform.position;
        PlayerRB.AddForce(RecallDirect.normalized * (ReverseRecallMultiplier * (pController._arrowController.MainArrow.recallSpeed * Time.fixedDeltaTime * 240)), ForceMode.Acceleration);
        LimitSpeed(pController._arrowController.MainArrow.MaxSpeed);

        RotatePlayer(RecallDirect, pController._stats.rotationSpeed / 2);
    }


    [Button]
    public void MeleeLunge()
    {
        Debug.Log("Lunge");
        // Use OverlapSphere to detect all colliders within the radius
        Collider[] hitColliders = Physics.OverlapSphere(PlayerRB.transform.position, lungeRange);

        // Variable to track the closest enemy within the field of view
        closestEnemy = null;
        closestDistance = Mathf.Infinity;
        lungeDirection = PlayerRB.transform.forward.normalized;  // Default to forward if no enemy is found

        // Loop through all colliders detected by the OverlapSphere
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;

            // Guard clause: If the hit object doesn't have the tag "Enemy", skip it
            if (!hitObject.CompareTag("Enemy")) continue;

            Vector3 directionToEnemy = (hitObject.transform.position - PlayerRB.transform.position).normalized;

            // Calculate the angle between the player's forward direction and the direction to the enemy
            float angleToEnemy = Vector3.Angle(PlayerRB.transform.forward, directionToEnemy);

            // Guard clause: If the enemy is outside the cone of ±30 degrees, skip it
            if (angleToEnemy > lungeAngle / 2 + rangeAngleTolerance) continue;

            // Calculate the distance to the enemy
            float distanceToEnemy = Vector3.Distance(PlayerRB.transform.position, hitObject.transform.position);

            // Guard clause: If this enemy is farther than the closest found, skip it
            if (distanceToEnemy >= closestDistance) continue;

            // Update closest enemy and distance if this one is closer
            closestEnemy = hitObject;
            closestDistance = distanceToEnemy;

            // Draw a line to this enemy for debug purposes
            Debug.DrawLine(PlayerRB.transform.position, hitObject.transform.position, Color.red, 1f);
        }

        // Guard clause: If no enemy was found, just lunge forward
        if (closestEnemy != null)
        {
            lungeDirection = (closestEnemy.transform.position - PlayerRB.transform.position).normalized;
            lungeDirection.y = 0;
        }

        doMeleeLunge(LungeTime);
    }

    public async UniTaskVoid doMeleeLunge(float time)
    {
        elapsedTime = 0f;  // Tracks how much time has passed

        while (elapsedTime < time)
        {
            PlayerRB.AddForce(lungeDirection * meleeLungeForce, ForceMode.Acceleration);
            RotatePlayer(lungeDirection, pController._stats.rotationSpeed * 0.75f);

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            elapsedTime += Time.deltaTime;
        }

        //Debug.Log("Lunge Rotation Complete");
    }

    #endregion


    private void OnDrawGizmos()
    {
        if (PlayerRB == null) return;

        //player facing
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(PlayerRB.transform.position, PlayerRB.transform.forward * lungeRange);

        // Calculate the boundaries of the lunge cone
        leftRayRotation = Quaternion.AngleAxis(-(lungeAngle / 2 + rangeAngleTolerance), Vector3.up);
        rightRayRotation = Quaternion.AngleAxis(lungeAngle / 2 + rangeAngleTolerance, Vector3.up);
        leftRayDirection = leftRayRotation * PlayerRB.transform.forward;
        rightRayDirection = rightRayRotation * PlayerRB.transform.forward;

        // Draw the cone boundaries
        Gizmos.color = Color.green;
        Gizmos.DrawRay(PlayerRB.transform.position, leftRayDirection * lungeRange);
        Gizmos.DrawRay(PlayerRB.transform.position, rightRayDirection * lungeRange);
    }
}
