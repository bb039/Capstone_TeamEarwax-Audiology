using TMPro;
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

    public void OnBackspacePress()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
