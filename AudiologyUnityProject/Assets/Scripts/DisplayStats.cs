using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class StatsSceneScript : MonoBehaviour
{
    public Transform contentParent;
    public GameObject statEntryRowPrefab;
    private string filePath;

    public string mainMenuScene;

    void Start()
    {
        filePath = Application.persistentDataPath + "/stats.json";
        LoadStats();
    }

    void LoadStats()
    {
        string path = Application.persistentDataPath + "/stats.json";

        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        StatsData statsData = JsonUtility.FromJson<StatsData>(json);

        for (int i = 0; i < statsData.playerNames.Count; i++)
        {
            GameObject row = Instantiate(statEntryRowPrefab, contentParent);

            string player = statsData.playerNames[i];
            string time = FormatTime(statsData.times[i]);

            Debug.Log($"Created entry: {player} - {time}");


            row.transform.Find("PlayerNameText").GetComponent<Text>().text = player;
            row.transform.Find("TimeText").GetComponent<Text>().text = time;

            row.SetActive(true);
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