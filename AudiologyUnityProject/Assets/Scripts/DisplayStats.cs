using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StatsSceneScript : MonoBehaviour
{
    public GameObject textPrefab;
    public Transform contentParent;
    //public Text statsText;
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
                for (int i = 0; i < statsData.playerNames.Count; i++)
                {
                    GameObject entry = Instantiate(textPrefab, contentParent);
                    entry.SetActive(true);

                    string formattedTime = FormatTime(statsData.times[i]);
                    entry.GetComponent<Text>().text = $"{statsData.playerNames[i]} - {formattedTime}";
                }
            }
            else
            {
                Debug.Log("StatsData is empty.");
                AddEmptyEntry("No stats recorded yet.");
            }
        }
        else
        {
            Debug.Log("Stats file not found: " + filePath);
            AddEmptyEntry("No stats recorded yet.");
        }
    }

    private void AddEmptyEntry(string message)
    {
        GameObject entry = Instantiate(textPrefab, contentParent);
        entry.SetActive(true);
        entry.GetComponent<Text>().text = message;
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