using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public string startGameScene;

    public void StartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }
}
