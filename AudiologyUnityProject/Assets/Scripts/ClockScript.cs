using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class ClockScript : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private float elapsedTime;
    private bool isRunning = false;

    public string endGameScene;

    [SerializeField] private GameObject statsManager;

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (elapsedTime >= 10f)
            {
                isRunning = false;
                statsManager.GetComponent<StatsManager>().setElapsedTime(elapsedTime);
                statsManager.GetComponent<StatsManager>().SaveCurrentRecord();
                SceneManager.LoadScene(endGameScene);
            }
        }
    }

    public void StartClock()
    {
        isRunning = true;
    }

    public bool IsClockRunning()
    {
        return isRunning;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
