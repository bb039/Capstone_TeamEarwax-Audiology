using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{
    public string mainMenuScene;
    public Text introText;
    
    private void Start()
    {
        GameObject statsManager = GameObject.Find("StatsManager");
        string name = statsManager.GetComponent<StatsManager>().getName();
        introText.text = $"Congratulations {name}! \r\nHere is how you did:";
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
