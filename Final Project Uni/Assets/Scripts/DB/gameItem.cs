using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class gameItem : ScriptableObject
{
    public string ID;
    public string itemName;
    public Sprite icon;
}
