//Only a class for PoolManager to control pool
//Interact with individual pool from PoolManager only

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Pool: MonoBehaviour
{
    //Stack to contain all GameObject of same type
    private Stack<GameObject> pooledObjects;
    //Game Object that we are pooling
    private GameObject prefab;

    //Contructor
    public Pool(GameObject prefab, int initialQty)
    {
        this.prefab = prefab;
        pooledObjects = new Stack<GameObject>(initialQty);
    }

    //get pool object count
    public int Count()
    {
        return pooledObjects.Count;
    }

    //take obj from pool but not place it in the world yet
    public GameObject TakeFromPool()
    {
        GameObject obj;
        if (pooledObjects.Count == 0)
        {
            obj = (GameObject)GameObject.Instantiate(prefab);
            obj.AddComponent<GameUnit>().pool = this;
            obj.SetActive(false);
        }
        else
        {
            obj = pooledObjects.Pop();
            //If null then call this method again to create a new Object
            if (obj == null)
            {
                return TakeFromPool();
            }
        }
        return obj;
    }
    public GameObject Spawn(Vector3 pos, Quaternion rot, float DespawnTime = 5f, Transform poolParent = null)
    {
        //Spawn an object , if no object in pool then create a object then add GameUnit Component to the object
        //and set pool to this
        //If there is object in pool then pop them out of the pool then SetActive(true)
        GameObject obj;
        if (pooledObjects.Count == 0)
        {
            obj = (GameObject)GameObject.Instantiate(prefab, pos, rot);
            obj.AddComponent<GameUnit>().pool = this;
        }
        else
        {
            //Take a GameObject from pool
            obj = pooledObjects.Pop();
            //If null then call this method again to create a new Object
            if (obj == null) return Spawn(pos, rot);
        }
        
        // Set parent instantly
        if (poolParent != null) obj.transform.SetParent(poolParent, true);

        //Set Active and Position/Rotation
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(true);
        
        //Despawn Setup here (TEST)
        //doDespawn(obj, DespawnTime);
        
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        doDespawn(obj);
    }

    public async UniTaskVoid doDespawn(GameObject obj, float despawnDelay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(despawnDelay));
        
        //push them back to the pool
        obj.SetActive(false);
        pooledObjects.Push(obj);
    }
}
