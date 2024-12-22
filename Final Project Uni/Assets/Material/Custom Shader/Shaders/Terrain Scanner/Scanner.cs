using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float speed = 10;
    public float Range = 20;
    public bool StartFromZero = true;
    private float growing;
    private Vector3 defaultScale;
    
    void Start()
    {
        if(StartFromZero) transform.localScale = Vector3.zero;

        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorMesh = transform.localScale;
        growing = speed * Time.deltaTime;
        transform.localScale = new Vector3(vectorMesh.x + growing, vectorMesh.y + growing, vectorMesh.z + growing);
        if (transform.localScale.x >= Range) SelfDestruct();
    }

    void SelfDestruct()
    {
        //use pool to replace :D
        transform.localScale = defaultScale;
        Destroy(gameObject);
    }
}
