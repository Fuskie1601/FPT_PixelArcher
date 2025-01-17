using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class IKSpiderAnimation : MonoBehaviour
{
    [FoldoutGroup("Calculate Setup")]
    public Transform[] legTargets;
    [FoldoutGroup("Calculate Setup")]
    public int smoothness = 8;
    [FoldoutGroup("Calculate Setup")]
    public float sphereCastRadius = 0.125f;
    [FoldoutGroup("Calculate Setup")]
    public float raycastRange = 1.5f;

    [FoldoutGroup("Settings")]
    public bool Calculate = true;
    [FoldoutGroup("Settings")]
    public float stepSize = 0.15f;
    [FoldoutGroup("Settings")]
    public float velocityMultiplier = 15f;
    [FoldoutGroup("Settings")]
    public AnimationCurve LegMoveCurve;
    [FoldoutGroup("Settings")]
    public float totalDuration = 0.15f;
    [FoldoutGroup("Settings")]
    public float stepHeight = 0.25f;
    [FoldoutGroup("Settings")]
    public bool GizmoDebugToggle;


    [SerializeField] private SpiderIKController _spiderMovement;

    private Vector3[] defaultLegPositions;
    private Vector3[] lastLegPositions;
    private Vector3 lastBodyUp;
    private bool[] legMoving;
    private int nbLegs;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private Vector3 lastBodyPos;

    Vector3[] MatchToSurfaceFromAbove(Vector3 point, float halfRange, Vector3 up)
    {
        Vector3[] res = new Vector3[2];
        res[1] = Vector3.zero;
        RaycastHit hit;
        Ray ray = new Ray(point + halfRange * up / 2f, -up);

        if (Physics.SphereCast(ray, sphereCastRadius, out hit, 2f * halfRange))
        {
            res[0] = hit.point;
            res[1] = hit.normal;
        }
        else
        {
            res[0] = point;
        }
        return res;
    }

    void Start()
    {
        lastBodyUp = _spiderMovement.GroundSurfaceNormal;

        nbLegs = legTargets.Length;
        defaultLegPositions = new Vector3[nbLegs];
        lastLegPositions = new Vector3[nbLegs];
        legMoving = new bool[nbLegs];
        for (int i = 0; i < nbLegs; ++i)
        {
            defaultLegPositions[i] = legTargets[i].localPosition;
            lastLegPositions[i] = legTargets[i].position;
            legMoving[i] = false;
        }
        lastBodyPos = transform.position;
    }

    IEnumerator PerformStep(int index, Vector3 targetPoint)
    {
        Vector3 startPos = lastLegPositions[index];
        lastLegPositions[index] = defaultLegPositions[index];

        float elapsedTime = 0f;

        while (elapsedTime < totalDuration && _spiderMovement.isGrounded)
        {
            float curveTime = elapsedTime / totalDuration;
            float yOffset = LegMoveCurve.Evaluate(curveTime) * stepHeight;

            Vector3 nextPos = Vector3.Lerp(startPos, targetPoint, curveTime);
            legTargets[index].position = nextPos + transform.up * yOffset;

            elapsedTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
            //yield return null; // Wait for the next frame
        }

        elapsedTime = 0f;
        while (elapsedTime < totalDuration && !_spiderMovement.isGrounded)
        {
            float curveTime = elapsedTime / totalDuration;
            Vector3 nextPos = Vector3.Lerp(startPos, targetPoint, curveTime);
            legTargets[index].position = nextPos;

            elapsedTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
            //yield return null;
        }

        legTargets[index].position = targetPoint;
        lastLegPositions[index] = legTargets[index].position;
        legMoving[0] = false;
        //MovementSound.Instance.PLaySoundAt(legTargets[index].position);
    }

    IEnumerator LegsOnAir()
    {
        float elapsedTime = 0f;
        Vector3[] startPositions = new Vector3[nbLegs];

        for (int i = 0; i < legTargets.Length; i++)
        {
            startPositions[i] = legTargets[i].position;
        }

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalDuration;// * 1.5f;

            for (int i = 0; i < nbLegs; i++)
            {
                legTargets[i].position = Vector3.Lerp(startPositions[i], transform.TransformPoint(defaultLegPositions[i]), t);
            }

            yield return new WaitForFixedUpdate();
            //yield return null;
        }

        // Ensure the legs reach the final default positions
        for (int i = 0; i < nbLegs; i++)
        {
            legTargets[i].position = transform.TransformPoint(defaultLegPositions[i]);
        }
    }

    void FixedUpdate()
    {
        Vector3 currentVelocity = _spiderMovement.transform.position - lastBodyPos;
        //Vector3 currentVelocity = _spiderMovement.rb.velocity;

        velocity = Vector3.Lerp(lastVelocity, currentVelocity, 1f / (smoothness + 1f));

        if (velocity.magnitude < 0.000025f)
            velocity = lastVelocity;
        else
            lastVelocity = velocity;

        //on Air
        Calculate = _spiderMovement.isGrounded;
        if (!Calculate)
        {
            //set all target at the position on air here
            StartCoroutine(LegsOnAir());
            lastBodyPos = _spiderMovement.transform.position;
            return;
        }

        Vector3[] desiredPositions = new Vector3[nbLegs];
        int indexToMove = -1;
        float maxDistance = stepSize;
        for (int i = 0; i < nbLegs; ++i)
        {
            desiredPositions[i] = transform.TransformPoint(defaultLegPositions[i]);

            float distance = Vector3.ProjectOnPlane(desiredPositions[i] + velocity * velocityMultiplier - lastLegPositions[i], -transform.up).magnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexToMove = i;
            }
        }
        for (int i = 0; i < nbLegs; ++i)
            if (i != indexToMove)
                legTargets[i].position = lastLegPositions[i];

        if (indexToMove != -1 && !legMoving[0])
        {
            Vector3 targetPoint = desiredPositions[indexToMove] + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0.0f, 1.5f) * (desiredPositions[indexToMove] - legTargets[indexToMove].position) + velocity * velocityMultiplier;

            // Calculate opposite direction from barrier
            Vector3 obstacleDirection = (targetPoint - legTargets[indexToMove].position).normalized;
            Vector3 newTargetPoint = targetPoint + obstacleDirection * (stepSize * 0.6f);

            Vector3[] positionAndNormalFwd = MatchToSurfaceFromAbove(newTargetPoint + velocity * velocityMultiplier, raycastRange, (_spiderMovement.GroundSurfaceNormal - velocity * 100).normalized);
            Vector3[] positionAndNormalBwd = MatchToSurfaceFromAbove(newTargetPoint + velocity * velocityMultiplier, raycastRange * (1f + velocity.magnitude), (_spiderMovement.GroundSurfaceNormal + velocity * 75).normalized);

            legMoving[0] = true;

            if (positionAndNormalFwd[1] == Vector3.zero)
                StartCoroutine(PerformStep(indexToMove, positionAndNormalBwd[0]));
            else
                StartCoroutine(PerformStep(indexToMove, positionAndNormalFwd[0]));
        }

        lastBodyPos = _spiderMovement.transform.position;

    }

    private void OnDrawGizmos()
    {
        if (!GizmoDebugToggle) return;
        for (int i = 0; i < nbLegs; ++i)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(legTargets[i].position, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.TransformPoint(defaultLegPositions[i]), stepSize);
        }
    }
}