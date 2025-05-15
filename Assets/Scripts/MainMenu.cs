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

    public void LevelEditor()
    {
        SceneManager.LoadSceneAsync("LevelEditorScene");
    }

    public void CreateVoidLevel()
    {
        PlayerPrefs.SetInt("CreateMode", 1);
        SceneManager.LoadScene("LevelEditorScene");
    }

    public void EditorChoice()
    {
        SceneManager.LoadSceneAsync("EditorChoiceScene");
    }

    public void EditLevel()
    {
        SceneManager.LoadSceneAsync("SelectLevelToEditScene");
    }
}
