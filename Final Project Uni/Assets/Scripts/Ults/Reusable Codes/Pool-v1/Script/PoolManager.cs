using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }
    private const int DEFAULT_POOL_SIZE = 30;
    private Dictionary<GameObject, Pool> poolDict;
    public Transform poolParent;
    public bool isRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Destroy duplicate instances
            return;
        }

        Instance = this;
        poolDict = new Dictionary<GameObject, Pool>();
        
        if(isRoot)
        DontDestroyOnLoad(gameObject);  // Make the PoolManager persistent across scenes if needed
    }

    // Initialize a Pool and put it inside poolDict
    private void InitPool(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
    {
        // Create a new pool and put it inside poolDict if there is no pool for the GameObject
        if (prefab != null && !poolDict.ContainsKey(prefab))
        {
            poolDict[prefab] = new Pool(prefab, qty);
        }
    }

    // Get pool count by prefab
    public int GetPoolCount(GameObject prefab)
    {
        if (poolDict.ContainsKey(prefab))
        {
            return poolDict[prefab].Count();
        }
        return 0;
    }

    // Spawn GameObject with prefab and position
    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, float DespawnTime = 5f)
    {
        // Create a new pool if no pool available with InitPool()
        InitPool(prefab);
        // Return the object from the pool's Spawn() method
        return poolDict[prefab].Spawn(pos, rot, DespawnTime, poolParent);
    }

    // Take an object from pool but not place in the game world yet
    public GameObject TakeFromPool(GameObject prefab)
    {
        InitPool(prefab);
        return poolDict[prefab].TakeFromPool();
    }

    // Despawn GameObject and return it to its pool
    public void Despawn(GameObject obj)
    {
        // Take the GameObject's GameUnit component. If null, the object was not from a pool
        GameUnit gameUnit = obj.GetComponent<GameUnit>();
        if (gameUnit == null)
        {
            Debug.LogWarning($"Object '{obj.name}' wasn't spawned from a pool. Destroying it instead.");
            Destroy(obj);
        }
        else
        {
            gameUnit.pool.Despawn(obj);
        }
    }

    // Preload objects into the pool
    public void Preload(GameObject prefab, int qty = 3, Transform poolParent = null)
    {
        // Create a new pool if no pool available
        InitPool(prefab, qty);

        // Pre-spawn objects and despawn them
        GameObject[] objs = new GameObject[qty];
        for (int i = 0; i < qty; i++)
        {
            objs[i] = Spawn(prefab, Vector3.zero, Quaternion.identity, 0);
        }

        for (int i = 0; i < qty; i++)
        {
            Despawn(objs[i]);
        }
    }
}
