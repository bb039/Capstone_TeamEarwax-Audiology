using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{
    public string mainMenuScene;
    public Text introText;
    public Text timeText;
    
    private void Start()
    {
        GameObject statsManager = GameObject.Find("StatsManager");

        string name = statsManager.GetComponent<StatsManager>().getName();
        float elapsedTime = statsManager.GetComponent<StatsManager>().getElapsedTime();

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        introText.text = $"Congratulations {name}! \r\nHere is how you did:";
        timeText.text = $"Time Taken: {formattedTime}";
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
