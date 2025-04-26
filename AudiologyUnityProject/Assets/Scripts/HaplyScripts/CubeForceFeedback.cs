using UnityEngine;
using TMPro; // if you are using TextMeshPro

public class CubeForceFeedback : MonoBehaviour
{
    [Header("Force Feedback Settings")]
    [Range(0, 800)] public float stiffness = 300f;
    [Range(0, 3)] public float damping = 1f;

    [Header("Visual Feedback")]
    public bool enableColorFeedback = false;
    public Color minForceColor = Color.white;
    public Color maxForceColor = Color.red;

    [Header("Pressure Warning UI")]
    public TextMeshProUGUI pressureWarningText; 
    public float pressureThreshold = 0.01f; 

    private Vector3 _cubePosition;
    private Vector3 _cubeSize;
    private Renderer _renderer;

    private float _penetration = 0f;

    private void Awake()
    {
        _cubePosition = transform.position;
        _cubeSize = transform.lossyScale;
        _renderer = GetComponent<Renderer>();

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
        _penetration = cursorRadius - distance;

        if (_penetration > 0)
        {
            Vector3 normal = distanceVector.normalized;
            force = normal * _penetration * stiffness;
            force -= cursorVelocity * damping;
        }
        else
        {
            _penetration = 0f;
        }

        return force;
    }

    private void Update()
    {
        if (enableColorFeedback && _renderer != null)
        {
            float normalized = Mathf.Clamp01(_penetration / 0.01f); // tweak denominator for sensitivity
            _renderer.material.color = Color.Lerp(minForceColor, maxForceColor, normalized);
        }

        if (pressureWarningText != null)
        {
            if (_penetration > pressureThreshold)
                pressureWarningText.gameObject.SetActive(true);
            else
                pressureWarningText.gameObject.SetActive(false);
        }
    }
}
