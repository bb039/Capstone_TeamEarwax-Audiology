using UnityEngine;

public class CubeForceFeedback : MonoBehaviour
{
    [Range(0, 800)]
    public float stiffness = 300f;

    [Range(0, 3)]
    public float damping = 1f;

    private Vector3 _cubePosition;
    private Vector3 _cubeSize;

    private void Awake()
    {
        _cubePosition = transform.position;
        _cubeSize = transform.lossyScale;

        // Register this cube with the HapticManager
        FindObjectOfType<HapticManager>().RegisterCube(this);
    }

    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        Vector3 force = Vector3.zero;

        Vector3 closestPoint = new Vector3(
            Mathf.Clamp(cursorPosition.x, _cubePosition.x - _cubeSize.x / 2f, _cubePosition.x + _cubeSize.x / 2f),
            Mathf.Clamp(cursorPosition.y, _cubePosition.y - _cubeSize.y / 2f, _cubePosition.y + _cubeSize.y / 2f),
            Mathf.Clamp(cursorPosition.z, _cubePosition.z - _cubeSize.z / 2f, _cubePosition.z + _cubeSize.z / 2f)
        );

        Vector3 distanceVector = cursorPosition - closestPoint;
        float distance = distanceVector.magnitude;
        float penetration = cursorRadius - distance;

        if (penetration > 0)
        {
            Vector3 normal = distanceVector.normalized;
            force = normal * penetration * stiffness;
            force -= cursorVelocity * damping;
        }

        return force;
    }
}
