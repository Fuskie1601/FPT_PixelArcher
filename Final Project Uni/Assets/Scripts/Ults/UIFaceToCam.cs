using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceToCam : MonoBehaviour
{
    // The target to track
    public Transform target;

    // Offset for the UI element's position relative to the target
    public Vector3 offset = Vector3.zero;

    // Smooth factor for movement (higher values will make it smoother/slower)
    public float smooth = 5f;

    private void LateUpdate()
    {
        // Make the UI element face the camera
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    private void Update()
    {
        if (target != null)
        {
            // Target position with offset
            Vector3 targetPosition = target.position + offset;

            // Smoothly move towards the target position using Lerp
            //transform.position = Vector3.Lerp(transform.position, targetPosition, smooth * Time.deltaTime);
            transform.position = targetPosition;
        }
    }
}