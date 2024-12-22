using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SeeThroughDetector : MonoBehaviour
{
    [FoldoutGroup("Stats")]
    public float MaskTime = 0.5f, unMaskTime = 0.25f, seeThroughSize = 5, sphereRadius = 0.5f;
    [FoldoutGroup("Stats")]
    public bool Activate;
    [FoldoutGroup("Debug")]
    public bool hitting;
    [FoldoutGroup("Debug/Setup")]
    public GameObject camera, target;
    [FoldoutGroup("Debug/Setup")]
    public LayerMask myLayerMask;

    // Calculate
    private RaycastHit hit;
    private Vector3 direction;
    private float distance;
    private bool isShrinking, isGrowing;

    void FixedUpdate()
    {
        if(!Activate) return;
        
        // Calculate the direction from the camera to the target
        direction = (target.transform.position - camera.transform.position).normalized;
        distance = Vector3.Distance(camera.transform.position, target.transform.position) - (sphereRadius * 2);

        // Perform a sphere cast using the calculated distance
        hitting = Physics.SphereCast(camera.transform.position, sphereRadius, direction, out hit, distance, myLayerMask);

        HitCheck(hitting);
    }

    public async UniTaskVoid HitCheck(bool hitting)
    {
        if (target == null)
        {
            Debug.LogWarning("Target has been destroyed. Aborting HitCheck.");
            return;
        }
        
        if (hitting && !isGrowing)
        {
            if (!hit.collider.gameObject.CompareTag("StencilMask"))
            {
                isShrinking = false;
                isGrowing = true;
                target.transform.DOScale(seeThroughSize, MaskTime).OnComplete(() => isGrowing = false);
            }
        }
        else if (!hitting && !isShrinking)
        {
            isGrowing = false;
            isShrinking = true;
            target.transform.DOScale(0, unMaskTime).OnComplete(() => isShrinking = false);
            await UniTask.Delay(TimeSpan.FromSeconds(unMaskTime));
        }
    }

    void OnDrawGizmos()
    {
        if (camera != null && target != null)
        {
            // Set the color for the Gizmos
            Gizmos.color = hitting ? Color.green : Color.red;
            // Draw the sphere cast line
            Gizmos.DrawLine(camera.transform.position, target.transform.position); // Draw a line from the camera to the target

            /*
             
            Gizmos.DrawSphere(camera.transform.position, sphereRadius);
            Gizmos.DrawSphere(target.transform.position, sphereRadius);
            if (hitting)
                Gizmos.DrawSphere(hit.point, sphereRadius);
             
            */
        }
    }
}
