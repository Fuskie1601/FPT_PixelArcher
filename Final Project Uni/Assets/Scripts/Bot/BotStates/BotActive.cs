using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;

public class BotActive : BaseState
{
    protected BotSM sm;
    protected Transform TF;
    public BotActive(string name, BotSM stateMachine) : base("Active", stateMachine)
    {
        sm = (BotSM)this.stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        TF = sm.bot.transform;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (sm.target != null)
        {
            //sm.bot.Distance = Vector3.Distance(sm.transform.position, sm.target.position);
            Rotate();
            //RotatePredict();
        }

    }
    public override void TriggerEnter(Collider other)
    {
        base.TriggerEnter(other);

        
        if (other.CompareTag("Player") && sm.target == null)
        {
            PlayerController c = other.gameObject.GetComponent<PlayerController>();
            if (c != null)
            {
                //sm.targets.Add(c.tf);
                sm.target = c.PlayerRB;
            }

        }
        if (other.CompareTag("Arrow"))
        {
            sm.ChangeState(sm.knockbackState);
            sm.bot.Health.Hurt(1);

        }
    }
    public void Rotate()
    {
        // Determine the direction to the target (only consider the X and Z axes)
        Vector3 targetDirection = sm.target.transform.position - TF.position;
        targetDirection.y = 0; // Ensure the direction is flat (only rotate on Y-axis)

        // The step size is equal to speed times frame time
        float singleStep = 8.0f * Time.deltaTime;

        // Calculate the new direction (XZ plane only)
        Vector3 newDirection = Vector3.RotateTowards(TF.forward, targetDirection, singleStep, 0.0f);

        // Apply the rotation only to the Y-axis
        Quaternion targetRotation = Quaternion.LookRotation(newDirection);
        TF.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Keep only the Y-axis rotation
    }


    public void RotatePredict()
    {
        // Check if the target has a Rigidbody to get the velocity
        Rigidbody targetRB = sm.target.GetComponent<Rigidbody>();
        if (targetRB == null) return; // If the target has no Rigidbody, skip

        // Get the target's velocity and predict its future position
        Vector3 targetVelocity = targetRB.velocity;
        Vector3 predictedPosition = sm.target.transform.position + targetVelocity * 1f; // 0.5f is a prediction factor, you can tweak it

        // Calculate the direction to the predicted position
        Vector3 targetDirection = predictedPosition - TF.position;

        // The step size is equal to rotation speed times frame time
        float singleStep = 8.0f * Time.deltaTime;

        // Rotate towards the predicted position, ignoring the Y axis to keep horizontal rotation
        Vector3 newDirection = Vector3.RotateTowards(TF.forward, new Vector3(targetDirection.x, 0, targetDirection.z), singleStep, 0.0f);

        // Apply the rotation to the bot
        TF.rotation = Quaternion.LookRotation(newDirection);
    }

}
