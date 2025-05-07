using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class DangerOverlayUI : MonoBehaviour
{
    public static DangerOverlayUI Instance;
    public AudioClip scream;
    [SerializeField]AudioSource audioData;
    [SerializeField] private Image overlayImage;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Color dangerColor = new Color(1f, 0f, 0f, 0.4f);

    private float _targetAlpha = 0f;
    private float _currentAlpha = 0f;
    private string _pendingMessage = "";

    private void Awake()
    {
        Instance = this;

        if (warningText != null)
        {
            warningText.alpha = 0f;
            warningText.text = "";  // Completely empty at start
        }

        if (overlayImage != null)
        {
            Color clearColor = dangerColor;
            clearColor.a = 0f;
            overlayImage.color = clearColor;
        }
    }

    public static void SetIntensity(float normalized, string message)
    {
        if (Instance == null) return;

        Instance._targetAlpha = Mathf.Clamp01(normalized);
        Instance._pendingMessage = message; 
    }

    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (overlayImage == null || warningText == null) return;

        _currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, Time.deltaTime * 10f);

        // Update vignette transparency
        Color newColor = dangerColor;
        newColor.a = Mathf.Lerp(0f, dangerColor.a, _currentAlpha);
        overlayImage.color = newColor;

        // Update warning text content and alpha safely
        if (_currentAlpha > 0.01f)
        {
            audioData.Play();
            warningText.text = _pendingMessage;  
            warningText.alpha = Mathf.Clamp01(_currentAlpha * 2f);
        }
        else
        {
            warningText.text = "";
            warningText.alpha = 0f;
        }
    }
}
