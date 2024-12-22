using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

//Basic Move Object
public class DynamicObstacle : MonoBehaviour
{
    [FoldoutGroup("Setup")]
    public Transform mesh;  // Transform of the obstacle to move
    [FoldoutGroup("Setup")]
    public List<HurtBox> HurtBoxes;  // List of HurtBoxes to toggle
    [FoldoutGroup("Setup")]
    public List<Transform> destinationList;  // List of destination positions

    [FoldoutGroup("Animation")]
    public Ease moveEase = Ease.InOutQuad;
    [FoldoutGroup("Animation")]
    public float moveTime;

    Vector3 defaultPos;
    
    private void Start()
    {
        // Save the initial position as the default position
        defaultPos = mesh.position;
    }

    public void ToggleHurtBox(bool toggle)
    {
        //Debug.Log("yes ? " + toggle);
        foreach (var hurtBox in HurtBoxes)
        {
            hurtBox.Activate = toggle;
        }
    }

    [Button]
    public void MoveToDestination(int destinationIndex)
    {
        if (destinationIndex >= 0 && destinationIndex < destinationList.Count)
        {
            Vector3 targetPos = destinationList[destinationIndex].position;
            mesh.DOMove(targetPos, moveTime).SetEase(moveEase);
        }
        else
            Debug.LogWarning("Invalid destination index");
    }
    [Button]
    public void MoveToDefault()
    {
        mesh.DOMove(defaultPos, moveTime).SetEase(moveEase);
    }
}