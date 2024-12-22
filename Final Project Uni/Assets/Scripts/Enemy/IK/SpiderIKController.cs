using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpiderIKController : MonoBehaviour
{
    [FoldoutGroup("Setup")]
    [SerializeField] private float groundDetectDistance;
    [FoldoutGroup("Setup")]
    [SerializeField] private LayerMask groundLayer;
    [FoldoutGroup("Setup")]
    public Transform SpiderCenter;
    [FoldoutGroup("Setup")]
    public Rigidbody rb;
    
    [FoldoutGroup("Debug and Calculate")] 
    public Vector3 GroundSurfaceNormal;
    [FoldoutGroup("Debug and Calculate")]
    [ReadOnly] public bool isGrounded;
    
    RaycastHit hit;

    private void FixedUpdate()
    {
        GroundCheck();
    }

    void GroundCheck()
    {
        //if (Physics.Raycast(SpiderCenter.position, -transform.up, out hit, groundDetectDistance, groundLayer))
        if (Physics.Raycast(SpiderCenter.position, new Vector3(0,-1,0), out hit, groundDetectDistance, groundLayer))
        {
            GroundSurfaceNormal = hit.normal;
            isGrounded = true;
        }
        else
        {
            GroundSurfaceNormal = Vector3.zero;
            isGrounded = false;
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw a red line in the Scene view to visualize the raycast direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(SpiderCenter.position, -GroundSurfaceNormal * groundDetectDistance);
    }
}
