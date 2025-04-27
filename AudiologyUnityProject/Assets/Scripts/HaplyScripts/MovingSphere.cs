using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [Range(0, 100)]
    public float pushForce = 0.01f;
    [Range(0, 100)]
    public float stickiness = 30f; // New: resist motion
    [Range(0, 10)]
    public float breakawayForce = 3f; // New: how hard to pull before unsticking

    private Rigidbody rb;
    private Vector3 cachedPosition;
    private Vector3 pendingForce;
    private float sphereRadius;
    private bool stuck = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereRadius = transform.lossyScale.x / 2f;
        FindObjectOfType<HapticManager>().RegisterMovingSphere(this);
    }

    private void Update()
    {
        cachedPosition = transform.position;

        if (pendingForce != Vector3.zero)
        {
            // If the ball is "stuck", apply resistive force
            if (stuck)
            {
                rb.AddForce(-rb.linearVelocity * stickiness, ForceMode.Force);
                
                // If pushing hard enough, break free
                if (rb.linearVelocity.magnitude > breakawayForce)
                {
                    stuck = false; // Now the ball moves normally
                }
            }

            rb.AddForce(pendingForce, ForceMode.Force);
            pendingForce = Vector3.zero;
        }
    }

    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        Vector3 force = Vector3.zero;

        Vector3 distanceVector = cursorPosition - cachedPosition;
        float distance = distanceVector.magnitude;
        float penetration = cursorRadius + sphereRadius - distance;

        if (penetration > 0)
        {
            Vector3 normal = distanceVector.normalized;

            // Clamp both upward and downward vertical force
            if (Mathf.Abs(normal.y) > 0.3f)
            {
                normal.y = Mathf.Sign(normal.y) * 0.3f; // Limit y component to ±0.3
                normal.Normalize();
            }

            force = normal * penetration * 100f;
            force -= cursorVelocity * 10f;

            pendingForce += -normal * pushForce;
        }



        return force;
    }
}
