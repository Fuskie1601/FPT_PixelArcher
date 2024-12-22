using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Gen/Room")]
[System.Serializable]
public class Room : ScriptableObject
{
    public string Name;
    public GameObject Prefab;

}
