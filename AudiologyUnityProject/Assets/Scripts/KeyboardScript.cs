using UnityEngine;
using UnityEngine.UI;

public class KeyboardScript : MonoBehaviour
{
    public InputField inputField;
    public Text textField;

    public void OnButtonPress()
    {
        inputField.text += textField.text;
    }
}
