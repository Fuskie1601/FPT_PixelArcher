using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRemover : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(RemovePlayer), 0.05f);
    }
    void RemovePlayer()
    {
        if(Player_Singleton.Instance != null)
            Destroy(Player_Singleton.Instance.gameObject);
    }
}
