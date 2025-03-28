using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public string startGameScene;
    public string statsScene;
    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }

    public void GoToStatsScene()
    {
        SceneManager.LoadScene(statsScene);
    }
}
