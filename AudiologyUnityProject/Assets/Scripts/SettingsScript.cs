using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] Text CerumenText;
    [SerializeField] Slider cerumenSlider;
    [SerializeField] Text CanalText;
    public void updateCerumen()
    {
        PlayerPrefs.SetInt("cerumenAmount", (int) cerumenSlider.value);
        CerumenText.text = $"Amount of Cerumen: {PlayerPrefs.GetInt("cerumenAmount")}";
    }

    public void updateHeadLeft()
    {
        PlayerPrefs.SetString("headType", "Left");
        CanalText.text = $"Canal Type: 1";
    }

    public void updateHeadCenter()
    {
        PlayerPrefs.SetString("headType", "Center");
        CanalText.text = $"Canal Type: 2";
    }

    public void updateHeadRight()
    {
        PlayerPrefs.SetString("headType", "Right");
        CanalText.text = $"Canal Type: 3";
        //CanalText.text = $"Canal Type: {PlayerPrefs.GetString("headType")}";
    }
}
