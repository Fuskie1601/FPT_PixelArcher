
using UnityEngine;

public class BotAttacking : BotTargeting
{
    private float hitCooldown = 0.3f;
    private float counter;

    public BotAttacking(BotSM stateMachine) : base("Attacking", stateMachine)
    {

    }
    public override void Enter()
    {
        base.Enter();
        counter = hitCooldown;
        sm.currState = "Attacking";
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (counter <= 0)
        {
            Attack();
            sm.ChangeState(sm.StrafeState);
        }
        else
        {
            counter -= Time.deltaTime;
            
            if(sm.bot._animController != null)
                sm.bot._animController.AttackAnim(false);
        }

    }
    public void Attack()
    {
        if (sm.bot.unitType == UnitType.BossShooter && sm.target != null)
        {
            sm.BossSkill.Attack(sm.target);
            return;
        }
        
        if (sm.target != null)
        {
            foreach (var gun in sm.bot.gun)
            {
                if(gun != null)
                gun.target = sm.target;
            }
            
            if(sm.bot._animController != null)
                sm.bot._animController.AttackAnim(true);
            
            counter = hitCooldown;
        }
    }
}
