using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class RoomGen : MonoBehaviour
{
    // The size of our world in grid cells.
    private int GridSize = 1;
    //private int Size = 100;
    private int MainPathLength = 4;
    private int Width = 20;
    private int Height = 20;
    private int SideRoomChance = 5;
    private List<Room> GenericRoom;
    private List<Room> PuzzleRoom;
    private List<Room> RewardRoom;
    private List<Room> BossRoom;
    private bool IsBossRoom = false;
    private Room StartRoom;
    private Room ExitRoom;
    private int TotalRewardRoom = 3;
    private int TotalPuzzleRoom = 3;
    // A 2D array that will store our collapsed tiles so we can reference them later.
    private Node[,] _grid;
    //List containing all possible nodes
    private List<Node> Nodes = new List<Node>();
    private List<GameObject> allRoom = new List<GameObject>();
    private Node origin = new Node();
    public Vector3 spawn;
    public GenerationManager gm;
    //An array of offset to make it easier to check neighbours
    // without duplicating code.
    private List<Vector2Int> sideOffsets = new List<Vector2Int>{
        new Vector2Int(0,1), //Top
        new Vector2Int(0,-1), //Bottom
        new Vector2Int(1,0), //Right
        new Vector2Int(-1,0) //Left
    };
    private List<Vector2Int> mainOffsets = new List<Vector2Int>{
        new Vector2Int(0,-1), //Bottom
        new Vector2Int(-1,0), //Left
        //new Vector2Int(-1,0) //Left
    };
    // public Vector3 GetSpawnPosition()
    // {
    //     return origin
    // }
    public void AssignData(int GridSize, int MainPathLength, int TotalPuzzleRoom, int TotalRewardRoom, List<Room> genericRoom, List<Room> rewardRoom, List<Room> puzzleRoom, List<Room> bossRoom, bool haveBoss, Room StartRoom, Room ExitRoom)
    {
        this.GridSize = GridSize;
        this.MainPathLength = MainPathLength;
        this.TotalPuzzleRoom = TotalPuzzleRoom;
        this.TotalRewardRoom = TotalRewardRoom;
        this.GenericRoom = genericRoom;
        this.RewardRoom = rewardRoom;
        this.PuzzleRoom = puzzleRoom;
        this.BossRoom = bossRoom;
        this.IsBossRoom = haveBoss;
        this.StartRoom = StartRoom;
        this.ExitRoom = ExitRoom;

    }


    public void Generate()
    {
        transform.localScale = Vector3.one;
        _grid = new Node[Width, Height];
        //First Pass 
        GenerateMainPath();
        //Instantiate the origin point
        GenerateSidePath();
        //Place room
        RoomPlace(TotalPuzzleRoom, PuzzleRoom);
        RoomPlace(TotalRewardRoom, RewardRoom);
        origin.ConnectedPos = CheckConnectedDirection(origin.currentPos, sideOffsets);
        //Debug.Log(origin.ConnectedPos.Count);
        GameObject o = GameObject.Instantiate(origin.room.Prefab, new Vector3(origin.currentPos.x * GridSize, 0f, origin.currentPos.y * GridSize), Quaternion.identity);
        spawn = new Vector3(origin.currentPos.x * GridSize, 2f, origin.currentPos.y * GridSize);
        o.GetComponent<RoomController>().SetConnector(origin.ConnectedPos);
        //o.transform.localScale = new Vector3(Size, 1, Size);
        o.transform.SetParent(this.transform);
        allRoom.Add(o);
        //Second Pass

        //Instantiate the main path and the side path
        foreach (Node n in origin.nodes)
        {
            n.ConnectedPos = CheckConnectedDirection(n.currentPos, sideOffsets);
            
            if(n.room == null) Debug.Log(n.Name + " " + n.room + " " + n.room.Prefab);
            GameObject g = GameObject.Instantiate(n.room.Prefab, new Vector3(n.currentPos.x * GridSize, 0f, n.currentPos.y * GridSize), Quaternion.identity);
            g.GetComponent<RoomController>().SetConnector(n.ConnectedPos);
            g.transform.SetParent(this.transform);
            allRoom.Add(g);
            if (n.nodes.Count > 0)
            {
                foreach (Node s in n.nodes)
                {
                    s.ConnectedPos = CheckConnectedDirection(s.currentPos, sideOffsets);
                    GameObject g1 = GameObject.Instantiate(s.room.Prefab, new Vector3(s.currentPos.x * GridSize, 0f, s.currentPos.y * GridSize), Quaternion.identity);
                    g1.GetComponent<RoomController>().SetConnector(s.ConnectedPos);
                    g1.transform.SetParent(this.transform);
                    allRoom.Add(g1);
                }
            }
        }
        //gm.SetScale();

    }
    [Button]
    public void Regenerate()
    {
        Clear();
        Generate();
    }
    public void Clear()
    {
        ClearGrid();  // Clears the grid data
        for (int i = 0; i < allRoom.Count; i++)
        {
            Destroy(allRoom[i]);  // Destroys all instantiated room GameObjects
        }
        allRoom.Clear(); // Clear the list of rooms
        origin = new Node(); // Reset the origin node
    }

    public void ClearGrid()
    {
        if (_grid != null)
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    _grid[x, y] = null; // Reset each grid cell
                }
            }
        }
    }
    private void GenerateMainPath()
    {
        origin.nodes.Clear();
        //Vector2Int pointer = new Vector2Int(Width / 2, Height / 2);
        Vector2Int pointer = new Vector2Int(Width / 2, Height / 2);
        origin.currentPos = pointer;
        origin.room = StartRoom;
        origin.IsSpecialRoom = true;
        _grid[pointer.x, pointer.y] = origin;
        Room EndRoom = ExitRoom;
        if (IsBossRoom)
        {
            EndRoom = BossRoom[Random.Range(0, BossRoom.Count)];
        }
        for (int i = 1; i <= MainPathLength; i++)
        {
            List<Vector2Int> ValidDir = CheckAvailableDirection(pointer, mainOffsets);
            if (ValidDir.Count > 0)
            {
                Vector2Int rV = ValidDir[Random.Range(0, ValidDir.Count)];
                pointer.x += rV.x;
                pointer.y += rV.y;
                Room p = GenericRoom[Random.Range(0, GenericRoom.Count)];
                bool sr = false;
                if (i == MainPathLength)
                {
                    p = EndRoom;
                    sr = true;
                }
                Node n = new Node()
                {
                    currentPos = pointer,
                    room = p,
                    IsSpecialRoom = sr
                };
                _grid[pointer.x, pointer.y] = n;
                origin.nodes.Add(n);

            }

        }
    }

    public void GenerateSidePath()
    {
        foreach (Node n in origin.nodes)
        {
            int roll = Random.Range(2, 11);
            //Debug.Log("Roll :" + roll);
            int chances = SideRoomChance;
            Vector2Int pointer = n.currentPos;
            //Debug.Log("Chances :" + chances);
            while (chances >= roll)
            {
                List<Vector2Int> ValidDir = CheckAvailableDirection(pointer, sideOffsets);
                //Debug.Log("aaaa" + offsets.Count);
                //Debug.Log(ValidDir.Count);
                //n.ConnectedPos = ValidDir;
                if (ValidDir.Count > 0)
                {
                    Vector2Int rV = ValidDir[Random.Range(0, ValidDir.Count)];
                    pointer.x += rV.x;
                    pointer.y += rV.y;
                    Node s = new Node()
                    {
                        currentPos = pointer,
                        room = GenericRoom[Random.Range(0, GenericRoom.Count)]

                    };
                    _grid[pointer.x, pointer.y] = s;
                    n.nodes.Add(s);
                }
                else
                {
                    break;
                }
                chances /= 2;

            }

        }
    }

    private void RoomPlace(int TotalRoom, List<Room> rooms)
    {
        int count = 0;
        int itr = 0;
        while (count < TotalRoom && itr <= 5)
        {
            itr++;
            foreach (Node n in origin.nodes)
            {
                if (count >= TotalRoom)
                {
                    break;
                }
                if (n.IsSpecialRoom == false && Random.Range(0, 3) > 1)
                {
                    n.room = rooms[Random.Range(0, rooms.Count)];
                    n.IsSpecialRoom = true;
                    count++;
                }
                if (n.nodes.Count > 0)
                {
                    foreach (Node s in n.nodes)
                    {
                        if (count >= TotalRoom)
                        {
                            break;
                        }
                        if (s.IsSpecialRoom == false && Random.Range(0, 3) > 1)
                        {
                            s.room = rooms[Random.Range(0, rooms.Count)];
                            s.IsSpecialRoom = true;
                            count++;
                        }
                    }
                }
            }
        }

    }
    private bool IsInsideGrid(Vector2Int v2int)
    {
        if (v2int.x > -1 && v2int.x < Width && v2int.y > -1 && v2int.y < Height)
        {
            return true;
        }
        return false;
    }
    private List<Vector2Int> CheckConnectedDirection(Vector2Int v2int, List<Vector2Int> offsets)
    {
        List<Vector2Int> ValidDir = offsets.ToList();


        for (int i = ValidDir.Count - 1; i >= 0; i--)
        {
            Vector2Int neighbour = new Vector2Int(v2int.x + ValidDir[i].x, v2int.y + ValidDir[i].y);
            // if (IsInsideGrid(neighbour) && _grid[neighbour.x, neighbour.y] == null)
            // {
            //     ValidDir.RemoveAt(i);
            // }
            if (!IsInsideGrid(neighbour) || _grid[neighbour.x, neighbour.y] == null)
            {
                ValidDir.RemoveAt(i);
            }
        }
        //Debug.Log(ValidDir.Count);
        return ValidDir;
    }
    private List<Vector2Int> CheckAvailableDirection(Vector2Int v2int, List<Vector2Int> offsets)
    {
        List<Vector2Int> ValidDir = offsets.ToList();


        for (int i = ValidDir.Count - 1; i >= 0; i--)
        {
            Vector2Int neighbour = new Vector2Int(v2int.x + ValidDir[i].x, v2int.y + ValidDir[i].y);
            if (!IsInsideGrid(neighbour) || _grid[neighbour.x, neighbour.y] != null)
            {
                ValidDir.RemoveAt(i);
            }
        }
        //Debug.Log(ValidDir.Count);
        return ValidDir;
    }
    // private List<Vector2Int> CheckConnectedRoom(Vector2Int v2int)
    // {
    //     List<Vector2Int> ValidDir = offsets.ToList();
    //     for (int i = ValidDir.Count - 1; i >= 0; i--)
    //     {
    //         Vector2Int neighbour = new Vector2Int(v2int.x + ValidDir[i].x, v2int.y + ValidDir[i].y);
    //         if (!IsInsideGrid(neighbour) || _grid[neighbour.x, neighbour.y] == null)
    //         {
    //             ValidDir.RemoveAt(i);
    //         }
    //     }
    //     return ValidDir;
    // }
}
