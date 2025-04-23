using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DangerOverlayUI : MonoBehaviour
{
    public static DangerOverlayUI Instance;

    [SerializeField] private Image overlayImage;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Color dangerColor = new Color(1f, 0f, 0f, 0.4f);

    private float _targetAlpha = 0f;
    private float _currentAlpha = 0f;

    private void Awake()
    {
        Instance = this;

        // Hide text at start
        if (warningText != null)
            warningText.alpha = 0f;
    }

    public static void SetIntensity(float normalized)
    {
        if (Instance == null) return;

        Instance._targetAlpha = Mathf.Clamp01(normalized);
    }

    private void Update()
    {
        if (overlayImage == null || warningText == null) return;

        _currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, Time.deltaTime * 10f);

        // Vignette
        Color newColor = dangerColor;
        newColor.a = Mathf.Lerp(0f, dangerColor.a, _currentAlpha);
        overlayImage.color = newColor;

        // Text fade in/out
        warningText.alpha = Mathf.Clamp01(_currentAlpha * 2f); // Text fades faster
    }
}
