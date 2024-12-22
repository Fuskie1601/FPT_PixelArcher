using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestructHandler : MonoBehaviour
{
    public delegate void DestroyedHandler(GameObject enemy);
    public event DestroyedHandler OnDestroyed;

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(gameObject);
    }
}
