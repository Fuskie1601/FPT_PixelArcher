using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomController : MonoBehaviour
{
    [System.Serializable]
    public struct DirectionConnector
    {
        public GameObject roomObject;
        public bool Invert;
    }

    [SerializeField] public NavMeshSurface navmeshSurface;
    [SerializeField] private List<DirectionConnector> UpConnections = new List<DirectionConnector>();
    [SerializeField] private List<DirectionConnector> DownConnections = new List<DirectionConnector>();
    [SerializeField] private List<DirectionConnector> LeftConnections = new List<DirectionConnector>();
    [SerializeField] private List<DirectionConnector> RightConnections = new List<DirectionConnector>();

    public void SetConnector(List<Vector2Int> connection)
    {
        if (connection.Contains(new Vector2Int(0, -1)))
        {
            SetDirectionActive(UpConnections, true);
        }
        if (connection.Contains(new Vector2Int(0, 1)))
        {
            SetDirectionActive(DownConnections, true);
        }
        if (connection.Contains(new Vector2Int(1, 0)))
        {
            SetDirectionActive(LeftConnections, true);
        }
        if (connection.Contains(new Vector2Int(-1, 0)))
        {
            SetDirectionActive(RightConnections, true);
        }
    }

    private void SetDirectionActive(List<DirectionConnector> connectors, bool isActive)
    {
        foreach (var connector in connectors)
        {
            if (connector.Invert)
            {
                // If inverted, do the opposite
                connector.roomObject.SetActive(!isActive);
            }
            else
            {
                // Normal behavior
                connector.roomObject.SetActive(isActive);
            }
        }
    }

    [Button]
    public void BuildNavMesh()
    {
        navmeshSurface.BuildNavMesh();
    }
}