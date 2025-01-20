using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void OpenSettings()
    {
        // SceneManager.LoadSceneAsync(?);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
