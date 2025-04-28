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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EditorChoice()
    {
        SceneManager.LoadSceneAsync("EditorChoiceScene");
    }

    public void CreateLevel()
    {
        SceneManager.LoadSceneAsync("CreateLevelScene");
    }

    public void EditLevel()
    {
        SceneManager.LoadSceneAsync("SelectLevelToEditScene");
    }
}
