using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


public class Node
{
    public string Name;
    public Room room;
    public RoomController rc;
    public bool IsSpecialRoom = false;
    public Vector2Int currentPos;
    public List<Vector2Int> ConnectedPos;
    public List<Node> nodes = new List<Node>();

}
