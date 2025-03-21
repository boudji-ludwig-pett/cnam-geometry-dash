using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadSceneAsync("SelectLevelScene");
    }

    public void OpenImportExport()
    {
        SceneManager.LoadSceneAsync("ImportExportScene");
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
