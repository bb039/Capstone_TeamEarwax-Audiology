using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameInputScript : MonoBehaviour 
{
    [SerializeField] InputField inputField;
    [SerializeField] Text resultText;
    [SerializeField] Button submitButton;
    [SerializeField] TimerScript timerScript;
    [SerializeField] GameObject statsManager;

    public void ValidateInput()
    {
        string input = inputField.text;

        if (!string.IsNullOrEmpty(input))
        {
            //resultText.text = input;
            statsManager.GetComponent<StatsManager>().setName(input);
            resultText.text = statsManager.GetComponent<StatsManager>().getName();
            inputField.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);

            timerScript.StartTimer();
            Time.timeScale = 1;
        }

   
    }
    
}
