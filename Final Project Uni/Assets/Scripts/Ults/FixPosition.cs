using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixPosition : MonoBehaviour
{
    //ở tỉ lệ FullHD
    //public Vector3 initialPosition = new Vector3(246.6f, 594.6f, 0);
    public Vector3 debugPosition;
    public Vector2 screenRatio = new Vector2(1920f, 1080f);
    //ti lệ so với màn hình
    public Vector2 screenPosition = new Vector2(0.09247496f, 0.605611f);

    // Start is called before the first frame update
    void Start()
    {
        // screenPosition = new Vector2(
        //     initialPosition.x / Screen.width,
        //     initialPosition.y / Screen.height
        // );

    }


    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(
            screenPosition.x * Screen.width,
            screenPosition.y * Screen.height,
            0
        );

        transform.position = newPosition;
        debugPosition = transform.position;
    }
}