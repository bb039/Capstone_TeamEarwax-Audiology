using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StatsSceneScript : MonoBehaviour
{
    public Text statsText;
    private string filePath;

    public string mainMenuScene;

    void Start()
    {
        filePath = Application.persistentDataPath + "/stats.json";
        LoadStats();
    }

    private void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("Loaded JSON: " + json);
            StatsData statsData = JsonUtility.FromJson<StatsData>(json);

            if (statsData != null && statsData.playerNames.Count > 0)
            {
                statsText.text = "Player Stats:\n";
                for (int i = 0; i < statsData.playerNames.Count; i++)
                {
                    string formattedTime = FormatTime(statsData.times[i]);
                    statsText.text += $"{statsData.playerNames[i]} - {formattedTime}\n";
                }
            }
            else
            {
                statsText.text = "No stats recorded yet.";
                Debug.Log("StatsData is empty.");
            }
        }
        else
        {
            statsText.text = "No stats recorded yet.";
            Debug.Log("Stats file not found: " + filePath);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}