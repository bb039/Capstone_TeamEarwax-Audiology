using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{
    public string mainMenuScene;

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
