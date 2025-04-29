using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [Range(0, 100)]
    public float pushForce = 0.01f;
    [Range(0, 100)]
    public float stickiness = 30f;
    [Range(0, 10)]
    public float breakawayForce = 3f;
    [Range(0, 1)]
    public float squishAmount = 0.2f;
    [Range(0, 10)]
    public float squishSpeed = 5f;

    public Transform visualSphere;

    private Rigidbody rb;
    private Vector3 cachedPosition;
    private Vector3 pendingForce;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private float sphereRadius;
    private bool stuck = true;

    private float currentSquishFactor = 0f;
    private float targetSquishFactor = 0f;

    private bool isTouching = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereRadius = transform.lossyScale.x / 2f;

        if (visualSphere == null)
        {
            visualSphere = transform.GetChild(0);
        }

        initialScale = visualSphere.localScale;
        targetScale = initialScale;

        FindObjectOfType<HapticManager>().RegisterMovingSphere(this);
    }

    private void Update()
    {
        cachedPosition = transform.position;

        if (pendingForce != Vector3.zero)
        {
            if (stuck)
            {
                rb.AddForce(-rb.linearVelocity * stickiness, ForceMode.Force);

                if (rb.linearVelocity.magnitude > breakawayForce)
                {
                    stuck = false;
                }
            }

            rb.AddForce(pendingForce, ForceMode.Force);
            pendingForce = Vector3.zero;
        }

        if (isTouching)
        {
            targetSquishFactor = 1f;
        }
        else
        {
            targetSquishFactor = 0f;
        }

        currentSquishFactor = Mathf.Lerp(currentSquishFactor, targetSquishFactor, squishSpeed * Time.deltaTime);
        ApplySquish(currentSquishFactor);
    }

    public Vector3 CalculateForce(Vector3 cursorPosition, Vector3 cursorVelocity, float cursorRadius)
    {
        Vector3 force = Vector3.zero;

        Vector3 distanceVector = cursorPosition - cachedPosition;
        float distance = distanceVector.magnitude;
        float penetration = cursorRadius + sphereRadius - distance;

        if (penetration > 0)
        {
            isTouching = true;

            Vector3 normal = distanceVector.normalized;

            if (Mathf.Abs(normal.y) > 0.3f)
            {
                normal.y = Mathf.Sign(normal.y) * 0.3f;
                normal.Normalize();
            }

            force = normal * penetration * 100f;
            force -= cursorVelocity * 10f;

            pendingForce += -normal * pushForce;
        }
        else
        {
            isTouching = false;
        }

        return force;
    }

    private void ApplySquish(float factor)
    {
        float lateralStretch = 1.0f + factor * squishAmount;
        float verticalSquash = 1.0f - factor * squishAmount;

        targetScale = new Vector3(
            initialScale.x * lateralStretch,
            initialScale.y * verticalSquash,
            initialScale.z * lateralStretch
        );

        visualSphere.localScale = targetScale;
    }
}
