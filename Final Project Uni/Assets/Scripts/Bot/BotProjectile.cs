using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BotProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rb;
    [ReadOnly] public float speed = 35f;

    private HurtBox _hurtBox;
    List<InteractTarget> validTargets = new List<InteractTarget>(); // List of valid targets
    private bool markedSelfDestruct;
    public Vector3 FlyVector;

    private void Awake()
    {
        _hurtBox = GetComponent<HurtBox>();
        _hurtBox.isProjectile = true;
        validTargets = _hurtBox.validTargets;
    }

    public void OnEnable()
    {
        markedSelfDestruct = true;
        
        // Set the velocity along the X and Z axes while keeping Y velocity zero to ensure the projectile stays level with the ground
        Vector3 forwardVelocity = transform.forward * speed;
        rb.velocity = new Vector3(forwardVelocity.x, 0, forwardVelocity.z);
        rb.transform.rotation = Quaternion.LookRotation(new Vector3(forwardVelocity.x, 0, forwardVelocity.z), Vector3.up);
        
        rb.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        //Invoke(nameof(SelfDestruct), 10f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(SelfDestruct));
        markedSelfDestruct = false;
    }

    public void SelfDestruct()
    {
        //Can add particle or sth here
        if(markedSelfDestruct)
        PoolManager.Instance.Despawn(gameObject);
    }
}
