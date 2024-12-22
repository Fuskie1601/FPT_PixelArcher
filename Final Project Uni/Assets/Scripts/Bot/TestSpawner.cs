using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //GameObject.Instantiate(prefab, spawnPos, transform.rotation);
        PoolManager.Instance.Spawn(prefab, spawnPos, transform.rotation);
    }


}
