using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    public Camera targetCamera;
    public bool useMainCamera = true;
    public Vector3 offset = Vector3.zero;

    private void Start()
    {
        if (useMainCamera)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera != null)
        {
            // For orthographic camera, we use the forward direction of the camera
            Vector3 cameraForward = targetCamera.transform.forward;

            // Ensure the object always faces the camera's forward direction
            transform.rotation = Quaternion.LookRotation(cameraForward);

            // Apply the offset rotation
            transform.Rotate(offset);
        }
        else
        {
            //Debug.LogWarning("LookAtCamera: No target camera assigned!");
            targetCamera = Camera.main;
        }
    }
}
