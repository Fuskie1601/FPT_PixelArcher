OBJECT POOLING PATTERN

+CREDIT :
    -Modified Object pooling by Martin "quill18" Glaude (quill18@quill18.com) SimplePool
    -Link to original : https://gist.github.com/quill18/5a7cfffae68892621267

+OVERVIEW:
    -Improve performance and memory use by reusing objects from a fixed pool instead of allocating and freeing them individually.
    -Reduce memory fragmentation and garbage collector spike by pre-allocating objects in memory
    -Check code and comment for implementation detail

+INSTRUCTION:
    -No special setup require
    -Recommend estimate the highest possible number of GameObject needed then use Pool.Preload()

+METHOD OVERVIEW:
    -Pool.Spawn(somePrefab, somePosition, someRotation);
        -Spawn an object with prefab(GameObject) , position(Vector 3) and rotation (Quaternion)
    -Pool.Despawn(GameObject);
        -SetActive(false) a GameObject in parameter and put them back in the pool
    -Pool.Preload(somePrefab, 20);
        -Preload object into a pool with quantity(int)

+TO-DO:
    -Add more functionality when needed in the future
    -Group all PoolObject inside GameObject
    -Need Unity Editor UI for ease of use

+CHANGES LOGS:
    -9/1/2024 : Created - Q
    -12/1/2024 : Implemented GetPoolCount() - Q
    -24/1/2024 : Implemented TakeFromPool() - Q

+MORE RESOURCE:
    - Object Pooling pattern overview : 
        -https://www.gameprogrammingpatterns.com/object-pool.html
    
