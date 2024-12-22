using UnityEngine;

public class ObjectChaser : MonoBehaviour
{
    // Enum to select the update method
    public enum UpdateMethod { FixedUpdate, Update, LateUpdate }
    public UpdateMethod updateMethod = UpdateMethod.Update;

    // Target to follow
    [SerializeField] private Transform target;

    // Toggle for smooth position movement (lerp)
    [SerializeField] private bool useLerp = true;

    // Smoothing factor for lerp
    [SerializeField] private float smoothValue = 5f;

    // Offset for position and rotation relative to the target's local space
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    // Update is called once per frame
    private void Update()
    {
        if (updateMethod == UpdateMethod.Update) FollowTarget();
    }

    // FixedUpdate is called at a fixed time interval
    private void FixedUpdate()
    {
        if (updateMethod == UpdateMethod.FixedUpdate) FollowTarget();
    }

    // LateUpdate is called after all Update functions have been called
    private void LateUpdate()
    {
        if (updateMethod == UpdateMethod.LateUpdate) FollowTarget();
    }

    // Method to follow the target with or without lerp, applying local position and rotation offsets
    private void FollowTarget()
    {
        if (target == null) return; // Guard clause to exit if no target is assigned

        // Calculate the local position and rotation based on the target's local space
        Vector3 targetPositionWithOffset = target.TransformPoint(positionOffset); // Transform positionOffset from local to world space
        Quaternion targetRotationWithOffset = target.rotation * Quaternion.Euler(rotationOffset); // Apply rotationOffset to the target's rotation

        // Position handling
        if (useLerp)
        {
            transform.position = Vector3.Lerp(transform.position, targetPositionWithOffset, smoothValue * Time.deltaTime);
        }
        else
        {
            transform.position = targetPositionWithOffset;
        }

        // Rotation handling
        if (useLerp)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationWithOffset, smoothValue * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotationWithOffset;
        }
    }
}
