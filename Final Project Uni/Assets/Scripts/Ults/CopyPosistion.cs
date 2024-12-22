using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode] 
public class CopyPosistion : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate the initial offset
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Update position relative to the target
        transform.position = target.position + offset;
    }
}
