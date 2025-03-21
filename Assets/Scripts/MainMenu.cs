using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadSceneAsync("SelectLevelScene");
    }

    public void OpenSettings()
    {
        // SceneManager.LoadSceneAsync(?);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LevelEditor()
    {
        SceneManager.LoadSceneAsync("LevelEditorScene");
    }
}
