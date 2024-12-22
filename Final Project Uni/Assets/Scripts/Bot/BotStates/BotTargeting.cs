using UnityEngine;

public class BotTargeting : BotActive
{

    public BotTargeting(string name, BotSM stateMachine) : base("BotTargeting", stateMachine)
    {

    }
    public override void Enter()
    {
        base.Enter();
        sm.currState = "Targeting";
        TF = sm.bot.transform;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (sm.target != null)
            Rotate();
    }

    public void Rotate()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = sm.target.transform.position - TF.position;
        // The step size is equal to speed times frame time.
        float singleStep = 2.0f * Time.deltaTime;
        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(TF.forward, targetDirection, singleStep, 0.0f);
        // Calculate a rotation a step closer to the target and applies rotation to this object
        TF.rotation = Quaternion.LookRotation(newDirection);
    }

}
