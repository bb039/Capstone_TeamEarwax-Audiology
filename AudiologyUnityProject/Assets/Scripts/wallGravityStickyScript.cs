using UnityEngine;

public class wallGravityStickyScript : MonoBehaviour
{
    private Rigidbody rb;
    float checkRadius;
    public LayerMask wallLayer;
    float velocity = 0f;

    public bool IsTouchingWall() {
        return Physics.CheckSphere(gameObject.transform.position, checkRadius, wallLayer);
    }
    public bool LogCollidingObjects()
    {
        // 1. Get an array of all colliders overlapping the sphere
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, wallLayer);

        // 2. Check if the array contains any colliders
        if (hitColliders.Length > 0)
        {
            // 3. Loop through the array and print the name of the object
            Debug.Log($"Object {gameObject.name} is colliding with {hitColliders.Length} wall(s):");

            foreach (Collider hitCollider in hitColliders)
            {
                // Access the GameObject from the Collider and print its name
                Debug.Log("    -> Wall Name: " + hitCollider.gameObject.name);
            }
            return true; // Collision detected
        }

        return false; // No collision
    }

    private void Start() {
        checkRadius = gameObject.transform.localScale[0];
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    private void Update() {
        if (IsTouchingWall()) {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            LogCollidingObjects();
        } else {
            rb.useGravity = true;
        }
    }
}
