using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementPrediction : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] [Range(0.1f, 5f)] private float historicalPositionDuration = 1f;
    [SerializeField] [Range(0.001f, 1f)] private float historicalPositionInterval = 0.1f;

    public Vector3 AverageVelocity
    {
        get
        {
            Vector3 average = Vector3.zero;
            foreach (Vector3 velocity in historicalVelocities)
            {
                average += velocity;
            }
            average.y = 0;

            return average / historicalVelocities.Count;
        }
    }

    private Queue<Vector3> historicalVelocities;
    private float lastPositionTime;
    private int maxQueueSize = 15;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        maxQueueSize = Mathf.CeilToInt(1f / historicalPositionInterval * historicalPositionDuration);
        historicalVelocities = new Queue<Vector3>(maxQueueSize);
    }

    private void Update()
    {
        if (lastPositionTime + historicalPositionInterval <= Time.time)
        {
            if (historicalVelocities.Count == maxQueueSize)
                historicalVelocities.Dequeue();

            historicalVelocities.Enqueue(rb.velocity);
            lastPositionTime = Time.time;
        }
    }
}
