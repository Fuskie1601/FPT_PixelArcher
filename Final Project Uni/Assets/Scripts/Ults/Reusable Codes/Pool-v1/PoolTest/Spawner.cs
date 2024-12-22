using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        //PoolManager.Preload(prefab, 5);
    }

    // Update is called once per frame
    void Update()
    {
        time = time - Time.deltaTime;
        if (time <= 0)
        {
            PoolManager.Instance.Spawn(prefab, this.transform.position, this.transform.rotation);
            //PoolManager.GetPoolCount(prefab);
            time = 2f;
        }
    }
}
