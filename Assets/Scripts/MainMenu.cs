using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadSceneAsync("SelectLevelScene");
    }

    public void OpenImport()
    {
        SceneManager.LoadSceneAsync("ImportScene");
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
