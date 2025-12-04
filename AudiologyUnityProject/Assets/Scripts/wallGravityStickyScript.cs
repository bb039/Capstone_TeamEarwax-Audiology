using UnityEngine;

public class wallGravityStickyScript : MonoBehaviour
{
    private Rigidbody rb;
    Vector3 checkRadius;
    public LayerMask wallLayer;
    float velocity = 0f;

    public bool IsTouchingWall() {
        return Physics.CheckBox(gameObject.transform.position, checkRadius, gameObject.transform.rotation, wallLayer);
    }
    public bool LogCollidingObjects()
    {
        // 1. Get an array of all colliders overlapping the sphere
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, checkRadius, gameObject.transform.rotation, wallLayer);

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
        checkRadius = gameObject.transform.localScale;
        checkRadius = new Vector3(checkRadius[0] / 2.0f, checkRadius[1] / 2.0f, checkRadius[2] / 2.0f);
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
