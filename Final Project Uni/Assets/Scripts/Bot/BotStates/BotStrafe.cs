using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotStrafe : BotActive
{
    public float wanderRadius = 25;
    public float wanderTimer = 5;

    private float timer, timeToPlayer, dot;
    private Vector3 predictedPosition, directionToPredicted, directionToPlayer, finalPosition;
    private PlayerMovementPrediction _playerMovePredict;
    
    public BotStrafe(BotSM stateMachine) : base("Strafe", stateMachine) {}
    public override void Enter()
    {
        base.Enter();
        sm.agent.isStopped = false;
        sm.currState = "Strafe";
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (sm.target != null && (sm.bot.unitType == UnitType.Shooter || sm.bot.unitType == UnitType.BossShooter))
        {
            sm.destination = ChooseAttackLocation(sm.target.position, sm.bot.minRange, sm.bot.maxRange, sm.bot.MoveAngle);
            sm.ChangeState(sm.chaseState);
        }

        if (sm.target != null && sm.bot.unitType == UnitType.Melee) 
            DoMoveToBlockPlayer(sm.target);
    }
    
    //basic
    void DoMoveToPlayer()
    {
        sm.destination = ChooseAttackLocation(sm.target.position, sm.bot.minRange, sm.bot.maxRange, sm.bot.MoveAngle);
        if (Vector3.Distance(TF.position, sm.destination) > sm.bot.attackRange)
        {
            sm.agent.SetDestination(sm.destination);
            
            if(sm.bot._animController != null)
                sm.bot._animController.UpdateRunInput(true);
        }
        else
        {
            sm.agent.isStopped = true;
            sm.ChangeState(sm.AttackingState);
        }
    }
    void DoMoveToBlockPlayer(Rigidbody player)
    {
        if (!sm.bot.UseMovementPrediction)
            DoMoveToPlayer();
        else
        {
            if (Vector3.Distance(TF.position, player.position) <= sm.bot.attackRange)
            {
                sm.agent.isStopped = true;
                sm.ChangeState(sm.AttackingState);
            }
                
            if(_playerMovePredict == null)
            _playerMovePredict = player.GetComponent<PlayerMovementPrediction>();
            if (_playerMovePredict == null)
            {
                DoMoveToPlayer();
                return;
            }
            
            if(sm.bot._animController != null)
                sm.bot._animController.UpdateRunInput(true);
            
            
            timeToPlayer = Vector3.Distance(player.transform.position, sm.transform.position) / sm.agent.speed;
            if (timeToPlayer > sm.bot.MovementPredictionTime) 
                timeToPlayer = sm.bot.MovementPredictionTime;
            
            // Calculate the predicted position based on the player's average velocity
            predictedPosition = player.position + _playerMovePredict.AverageVelocity * timeToPlayer;
            directionToPredicted = (predictedPosition - sm.transform.position).normalized;
            directionToPlayer = (player.position - sm.transform.position).normalized;

            // Dot product to decide if the bot should block the predicted position or move directly to the player
            dot = Vector3.Dot(directionToPlayer, directionToPredicted);
            finalPosition = dot < sm.bot.MovementPredictionThreshold ? player.position : predictedPosition;

            sm.agent.SetDestination(finalPosition);
        }
    }

    
    public Vector3 ChooseAttackLocation(Vector3 targetPosition, float minRadius, float maxRadius, float halfAngle = 45f)
    {
        // The enemy's current position
        Vector3 enemyPosition = sm.transform.position;

        // Calculate the direction from the target to the enemy
        Vector3 directionToEnemy = (enemyPosition - targetPosition).normalized;

        // Pick a random angle within the fan (between -halfAngle and +halfAngle)
        float randomAngle = Random.Range(-halfAngle, halfAngle);

        // Rotate the direction vector by the random angle (in degrees) around the Y axis
        Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * directionToEnemy;

        // Pick a random distance within the min and max radius
        float randomDistance = Random.Range(minRadius, maxRadius);

        // Calculate the final point within the fan
        Vector3 point = targetPosition + randomDirection * randomDistance;

        // Return the calculated point
        return new Vector3(point.x, enemyPosition.y, point.z); // Keep the Y position of the enemy
    }
    
    // Method to warp the bot behind the player
    public void MoveToBehindPlayer(float offset)
    {
        if (sm.target == null) return;

        // Get the player's position and forward direction
        Vector3 playerPosition = sm.target.position;
        Vector3 playerForward = sm.target.transform.forward;

        // Calculate the position behind the player with the given offset
        Vector3 behindPlayerPosition = playerPosition - playerForward * offset;
        Vector3 telePos = new Vector3(behindPlayerPosition.x, sm.transform.position.y, behindPlayerPosition.z);

        // Warp the bot to the calculated position
        sm.Teleport(telePos);

        // Optional: Update animation to reflect the warp
        if(sm.bot._animController != null)
            sm.bot._animController.UpdateRunInput(false);
    }
}
