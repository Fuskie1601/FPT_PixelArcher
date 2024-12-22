using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Singleton : MonoBehaviour
{
    public static Manager_Singleton Instance;

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
