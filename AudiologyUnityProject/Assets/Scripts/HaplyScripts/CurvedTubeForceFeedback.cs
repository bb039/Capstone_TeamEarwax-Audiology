using UnityEngine;
using System.Collections.Generic;

public class CurvedTubeForceFeedback : MonoBehaviour
{
    [Header("Spline Tube Settings")]
    public List<Transform> controlPoints;  // Original transforms
    public AnimationCurve radiusProfile;
    public float splineResolution = 100;

    [Header("Force Feedback")]
    public float stiffness = 300f;
    public float damping = 1f;
    public bool enableForce = true;

    [Header("Debug")]
    public bool visualizeClosestPoint = true;

    private Vector3[] cachedPositions;  // <-- cache
    private Vector3 _closestPoint;
    private float _penetration;

    private void Awake()
    {
        CacheControlPointPositions();
        FindObjectOfType<HapticManager>().RegisterTube(this);
    }

    private void CacheControlPointPositions()
    {
        cachedPositions = new Vector3[controlPoints.Count];
        for (int i = 0; i < controlPoints.Count; i++)
        {
            cachedPositions[i] = controlPoints[i].position;
        }
    }

    public Vector3 CalculateForce(Vector3 cursorPos, Vector3 cursorVel, float cursorRadius)
    {
        _penetration = 0f;
        Vector3 force = Vector3.zero;

        float closestDist = float.MaxValue;
        Vector3 bestPoint = Vector3.zero;
        float bestT = 0f;

        for (int i = 0; i <= splineResolution; i++)
        {
            float t = i / (float)splineResolution;
            Vector3 pointOnSpline = GetPointOnSpline(t);
            float dist = Vector3.Distance(cursorPos, pointOnSpline);

            if (dist < closestDist)
            {
                closestDist = dist;
                bestPoint = pointOnSpline;
                bestT = t;
            }
        }

        _closestPoint = bestPoint;
        float wallRadius = radiusProfile.Evaluate(bestT);
        float distanceFromCenterline = Vector3.Distance(cursorPos, bestPoint);

        _penetration = distanceFromCenterline - (wallRadius - cursorRadius);

        if (_penetration > 0 && enableForce)
        {
            Vector3 normal = (bestPoint - cursorPos).normalized; // PUSH BACK toward center
            force = normal * _penetration * stiffness;
            force -= cursorVel * damping;
        }

        return force;
    }


    // Catmull-Rom spline interpolation but using cached positions
    private Vector3 GetPointOnSpline(float t)
    {
        if (cachedPositions == null || cachedPositions.Length < 4)
            return Vector3.zero;

        int numSections = cachedPositions.Length - 3;
        t = Mathf.Clamp01(t) * numSections;
        int currIndex = Mathf.FloorToInt(t);
        t -= currIndex;

        int p0 = Mathf.Clamp(currIndex, 0, cachedPositions.Length - 1);
        int p1 = Mathf.Clamp(currIndex + 1, 0, cachedPositions.Length - 1);
        int p2 = Mathf.Clamp(currIndex + 2, 0, cachedPositions.Length - 1);
        int p3 = Mathf.Clamp(currIndex + 3, 0, cachedPositions.Length - 1);

        return 0.5f * (
            (2f * cachedPositions[p1]) +
            (-cachedPositions[p0] + cachedPositions[p2]) * t +
            (2f * cachedPositions[p0] - 5f * cachedPositions[p1] + 4f * cachedPositions[p2] - cachedPositions[p3]) * t * t +
            (-cachedPositions[p0] + 3f * cachedPositions[p1] - 3f * cachedPositions[p2] + cachedPositions[p3]) * t * t * t
        );
    }


    private void OnDrawGizmos()
    {
        if (cachedPositions == null || cachedPositions.Length < 4)
            return;

        Gizmos.color = Color.green;

        int samples = 100;
        Vector3 prevPoint = GetPointOnSpline(0);

        for (int i = 1; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector3 currPoint = GetPointOnSpline(t);

            // Draw centerline
            Gizmos.DrawLine(prevPoint, currPoint);

            prevPoint = currPoint;
        }

        // Draw tube cross sections (optional)
        Gizmos.color = Color.cyan;
        for (int i = 0; i <= samples; i += 10)
        {
            float t = i / (float)samples;
            Vector3 point = GetPointOnSpline(t);
            float radius = radiusProfile != null ? radiusProfile.Evaluate(t) : 0.01f;

            Gizmos.DrawWireSphere(point, radius);
        }

        // Draw closest point to cursor (optional)
        if (visualizeClosestPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_closestPoint, 0.002f);
        }
    }

}
