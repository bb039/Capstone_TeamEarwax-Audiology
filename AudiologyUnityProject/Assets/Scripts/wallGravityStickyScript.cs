using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EarWaxGunkScript : MonoBehaviour
{
    public enum WaxState { Frozen, FreeFloating, StuckToTool }

    [Header("Settings")]
    public LayerMask wallLayer;
    public LayerMask toolLayer;
    public string waxTag = "Wax";
    
    [Header("Tool Interaction")]
    public string stickySurfaceTag = "StickyTarget"; 
    public float snapForce = 0.05f;

    [Header("Physics Tuning")]
    // If the wax is moving slower than this, it decides it's safe to freeze again.
    public float freezeThreshold = 0.1f; 
    
    private Rigidbody rb;
    private Collider myCollider;
    public WaxState currentState = WaxState.Frozen;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        // SAFETY: Prevent explosions when objects overlap
        rb.maxDepenetrationVelocity = 1.0f;

        Freeze();
    }

    private void FixedUpdate()
    {
        // LOGIC: If we are floating (pushed by tool), try to Stick/Freeze back to the cluster
        if (currentState == WaxState.FreeFloating)
        {
            // If we are moving very slowly, or moving away from the tool, try to lock down
            if (rb.linearVelocity.magnitude < freezeThreshold)
            {
                if (IsTouchingStableSupport())
                {
                    Freeze();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == WaxState.StuckToTool) return;

        // 1. TOOL INTERACTION
        if (IsInLayer(collision.gameObject, toolLayer))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // A. STICKY PART -> Parent it
                if (contact.otherCollider.CompareTag(stickySurfaceTag))
                {
                    if (CheckLineOfSight(contact.point, contact.otherCollider))
                    {
                        AttachToTool(collision);
                        return;
                    }
                }
            }

            // B. ROD/HANDLE -> Just wake up so we can be pushed
            Unfreeze();
        }

        // 2. WALL INTERACTION -> Always freeze on walls
        else if (IsInLayer(collision.gameObject, wallLayer))
        {
            Freeze();
        }
        
        // 3. WAX NEIGHBOR -> If they are frozen, I should freeze too
        else if (collision.gameObject.CompareTag(waxTag))
        {
            EarWaxGunkScript neighbor = collision.gameObject.GetComponent<EarWaxGunkScript>();
            if (neighbor != null && neighbor.currentState == WaxState.Frozen)
            {
                // If I hit a stable block, I stop moving.
                // This prevents the "Puddle" domino effect.
                Freeze();
            }
        }
    }

    // --- SUPPORT CHECK ---
    private bool IsTouchingStableSupport()
    {
        // Check slightly larger than ourself to see if we are near a wall or frozen block
        float checkRadius = transform.localScale.x * 0.6f;
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            // Wall? Stable.
            if (IsInLayer(hit.gameObject, wallLayer)) return true;

            // Frozen Wax? Stable.
            if (hit.CompareTag(waxTag))
            {
                EarWaxGunkScript s = hit.GetComponent<EarWaxGunkScript>();
                if (s != null && s.currentState == WaxState.Frozen) return true;
            }
        }
        return false;
    }

    // --- ACTIONS ---

    private void Freeze()
    {
        currentState = WaxState.Frozen;
        rb.useGravity = false;
        rb.isKinematic = false; // Keep physics logic running...
        rb.constraints = RigidbodyConstraints.FreezeAll; // ...but lock position.
        rb.linearVelocity = Vector3.zero;
        
        // Clean up tool parenting if needed
        if (transform.parent != null && transform.parent.gameObject.layer == LayerMask.NameToLayer("ToolLayer")) // Optional check
        {
            transform.SetParent(null);
        }
    }

    private void Unfreeze()
    {
        if (currentState == WaxState.StuckToTool) return;

        currentState = WaxState.FreeFloating;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true; 
        rb.isKinematic = false;
    }

    private void AttachToTool(Collision collision)
    {
        currentState = WaxState.StuckToTool;
        
        // "Simcade" Parenting -> 100% Stability
        rb.isKinematic = true; 
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        SnapToSurface(collision);
        transform.SetParent(collision.gameObject.transform);
    }

    // --- UTILITIES ---

    private bool CheckLineOfSight(Vector3 targetPoint, Collider targetCollider)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = targetPoint - startPos;
        RaycastHit hit;
        // Cast against Tool Layer
        if (Physics.Raycast(startPos, direction.normalized, out hit, direction.magnitude + 0.1f, toolLayer))
        {
            if (hit.collider == targetCollider) return true;
            if (hit.collider.CompareTag(stickySurfaceTag) == false) return false; 
        }
        return true;
    }

    private void SnapToSurface(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 direction = (contact.point - transform.position).normalized;
            transform.position += direction * snapForce;
        }
    }

    private bool IsInLayer(GameObject obj, LayerMask mask) { return ((1 << obj.layer) & mask) != 0; }
}

// using UnityEngine;

// public class wallGravityStickyScript : MonoBehaviour
// {
//     private Rigidbody rb;
//     Vector3 checkRadius;
//     public LayerMask wallLayer;
//     float velocity = 0f;

//     public bool IsTouchingWall() {
//         return Physics.CheckBox(gameObject.transform.position, checkRadius * 1.01f, gameObject.transform.rotation, wallLayer);
//     }
//     public bool LogCollidingObjects()
//     {
//         // 1. Get an array of all colliders overlapping the sphere
//         Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, checkRadius, gameObject.transform.rotation, wallLayer);

//         // 2. Check if the array contains any colliders
//         if (hitColliders.Length > 0)
//         {
//             // 3. Loop through the array and print the name of the object
//             Debug.Log($"Object {gameObject.name} is colliding with {hitColliders.Length} wall(s):");
//             foreach (Collider hitCollider in hitColliders)
//             {
//                 // Access the GameObject from the Collider and print its name
//                 Debug.Log("    -> Wall Name: " + hitCollider.gameObject.name);
//             }
//             return true; // Collision detected
//         }

//         return false; // No collision
//     }

//     private void Start() {
//         checkRadius = gameObject.transform.localScale;
//         checkRadius = new Vector3(checkRadius[0] / 2.0f, checkRadius[1] / 2.0f, checkRadius[2] / 2.0f);
//         rb = GetComponent<Rigidbody>();
//         rb.useGravity = false;
//     }
//     private void Update() {
//         if (IsTouchingWall()) {
//             rb.useGravity = false;
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//             LogCollidingObjects();
//         } else {
//             rb.useGravity = true;
//         }
//     }
// }
