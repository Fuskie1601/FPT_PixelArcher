using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;

public class BotKnockback : BotDisable
{
    // Cooldown for knockback
    protected float cooldown = 1f;
    protected float countdown;

    public Vector3 force;
    NavMeshHit hit;

    // Knockback thresholds and timers
    private const float MaxKnockbackTime = 2.0f;  // Maximum time knockback can last
    private const float StillThreshold = 0.1f;    // Velocity threshold to stop knockback

    private bool damaged;

    private bool isKnockbackActive = false;

    public BotKnockback(BotSM stateMachine) : base("Knockback", stateMachine)
    {
        sm = (BotSM)this.stateMachine;
        force = Vector3.zero;
    }

    public override void Enter()
    {
        base.Enter();
        sm.currState = "Knockback";
        countdown = cooldown;
        
        if(sm.bot._animController != null)
            sm.bot._animController.UpdateRunInput(false);

        applyKnockback(force);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        // After cooldown, transition to the StrafeState if knockback has completed
        if (countdown >= 0)
        {
            countdown -= Time.deltaTime;

            if (sm.bot._animController != null && sm.isAlive && !damaged)
            {
                damaged = true;
                sm.bot._animController.DamagedAnim();
            }
        }
        else if (!isKnockbackActive) // Ensure knockback is finished before changing state
        {
            damaged = false;
            if(sm.isAlive) sm.ChangeState(sm.StrafeState);
        }
    }

    private async UniTask applyKnockback(Vector3 force)
    {
        // Mark knockback as active
        isKnockbackActive = true;

        // Disable agent and enable physics-based movement
        sm.agent.enabled = false;
        sm.bot.rg.useGravity = true;
        sm.bot.rg.isKinematic = false;

        // Apply knockback force
        Vector3 knockbackForce = new Vector3(force.x, 5, force.z);  // Adjusted knockback with some vertical force
        sm.bot.rg.AddForce(knockbackForce, ForceMode.Impulse);

        // Wait for a fixed frame
        await UniTask.WaitForFixedUpdate();

        // Capture the start time
        float knockbackStartTime = Time.time;

        // Wait until the knockback completes (either by velocity or max knockback time)
        await UniTask.WaitUntil(() =>
            sm.bot.rg.velocity.magnitude < StillThreshold || Time.time > knockbackStartTime + MaxKnockbackTime
        );

        // Optional: Small delay before regaining control
        await UniTask.Delay(250); // 250 ms

        // Reset physics settings
        if(sm.bot.rg == null) return;
        sm.bot.rg.velocity = Vector3.zero;
        sm.bot.rg.angularVelocity = Vector3.zero;
        sm.bot.rg.useGravity = false;
        sm.bot.rg.isKinematic = true;

        // Sample the NavMesh to find the closest valid point
        bool foundPosition = NavMesh.SamplePosition(sm.bot.transform.position, out hit, 100f, NavMesh.AllAreas);

        if (foundPosition)
        {
            // Calculate the direction from the bot's current position to the hit point
            Vector3 directionToHit = (hit.position - sm.bot.transform.position).normalized;

            // Apply an offset along the X and Z axes to go "deeper" in the direction to the hit point
            Vector3 deeperPosition = hit.position + new Vector3(directionToHit.x, 0, directionToHit.z) * 5f;

            // Warp the bot to the new deeper position on the NavMesh
            sm.agent.Warp(deeperPosition);
        }
        else
        {
            // Fallback to the current position if no valid point was found
            sm.agent.Warp(sm.bot.transform.position);
        }

        // Re-enable the NavMeshAgent
        sm.agent.enabled = true;

        // Mark knockback as completed
        isKnockbackActive = false;
    }

}
