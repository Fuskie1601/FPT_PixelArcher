using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AimType
{
    Basic, Predict, AccuratePredict, RandomOffset
}

public class BotGun : MonoBehaviour
{
    public bool Activated = true;
    public AimType aimType;
    public GameObject projectile;
    public List<ParticleSystem> muzzleFlash;
    public Rigidbody target;
    public float predictionFactor = 0.5f, projectileSpeed = 35f, randomAngleRange = 5f;

    private BotProjectile projectileCalc;

    private void Awake()
    {
        projectileCalc = projectile.GetComponent<BotProjectile>();
        if (projectileCalc != null)
            projectileCalc.speed = projectileSpeed;
    }

    public void Fire()
    {
        if(!Activated) return;
        
        switch (aimType)
        {
            case AimType.Basic:
                FireStraight();
                break;
            case AimType.Predict:
                FirePredict();
                break;
            case AimType.AccuratePredict:
                FirePredictHighAccurate();
                break;
            case AimType.RandomOffset:
                FireRandomOffset();
                break;
        }
    }

    public void PlayMuzzleFlash(int slot = 0)
    {
        muzzleFlash[slot].Play();
    }

    // Regular fire method (straight shooting)
    public void FireStraight()
    {
        // Get the world space rotation directly (without local rotation adjustments)
        Quaternion worldRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        
        projectileCalc.FlyVector = transform.forward;
        // Spawn the projectile with the world space rotation
        PoolManager.Instance.Spawn(projectile, transform.position, worldRotation);
    }
    public void FireRandomOffset()
    {
        // Get the world space rotation for forward direction
        Quaternion baseRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        // Add random offset to the Y-axis
        float randomYRotation = Random.Range(-randomAngleRange, randomAngleRange);
        Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0) * baseRotation;


        projectileCalc.FlyVector = transform.forward;
        // Spawn the projectile with the random rotation
        PoolManager.Instance.Spawn(projectile, transform.position, randomRotation);
    }
    

    // predictive shooting method
    public void FirePredict()
    {
        if (target == null) return;

        // Get the target's velocity and predict the future position based on that velocity
        Vector3 targetVelocity = target.velocity;
        Vector3 predictedPosition = target.position + new Vector3(targetVelocity.x, 0, targetVelocity.z) * predictionFactor;

        // Calculate the direction to the predicted position (ignoring Y-axis for horizontal shots)
        Vector3 targetDirection = predictedPosition - transform.position;
        targetDirection.y = 0; // Ensure the projectile stays parallel to the ground

        // Calculate the Y-axis rotation (horizontal plane only)
        Vector3 flattenedDirection = new Vector3(targetDirection.x, 0, targetDirection.z); // Flatten the direction to ignore Y-axis
        Quaternion predictedRotation = Quaternion.LookRotation(flattenedDirection, Vector3.up); // Look at the target only on the Y-axis

        projectileCalc.FlyVector = transform.forward;
        // Spawn the projectile with the predicted rotation
        PoolManager.Instance.Spawn(projectile, transform.position, predictedRotation);
    }
    public void FirePredictHighAccurate()
    {
        if (target == null) return;

        // Get the target's velocity
        Vector3 targetVelocity = target.velocity;

        // Calculate the distance to the target
        Vector3 distanceToTarget = target.position - transform.position;
        float distanceMagnitude = distanceToTarget.magnitude;

        // Calculate the time it will take for the projectile to reach the target's current position
        float timeToReachTarget = distanceMagnitude / projectileSpeed;

        // Predict the future position of the target considering the time it takes for the projectile to reach them
        Vector3 predictedPosition = target.position + (targetVelocity * predictionFactor) * timeToReachTarget;

        // Calculate the direction to the predicted position (ignoring Y-axis for horizontal shots)
        Vector3 targetDirection = predictedPosition - transform.position;
        targetDirection.y = 0; // Ensure the projectile stays parallel to the ground

        // Calculate the Y-axis rotation (horizontal plane only)
        Vector3 flattenedDirection = new Vector3(targetDirection.x, 0, targetDirection.z); // Flatten the direction to ignore Y-axis
        Quaternion predictedRotation = Quaternion.LookRotation(flattenedDirection, Vector3.up); // Look at the target only on the Y-axis

        projectileCalc.FlyVector = transform.forward;
        // Spawn the projectile with the predicted rotation
        PoolManager.Instance.Spawn(projectile, transform.position, predictedRotation);
    }

}
